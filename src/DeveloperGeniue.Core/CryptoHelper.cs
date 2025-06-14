using System.Security.Cryptography;
using System.Text;

namespace DeveloperGeniue.Core;

public static class CryptoHelper
{
    public static string Encrypt(string plaintext, string? passphrase)
    {
        if (OperatingSystem.IsWindows())
        {
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        if (string.IsNullOrEmpty(passphrase))
            throw new InvalidOperationException("Passphrase required for encryption on this platform.");

        using var aes = Aes.Create();
        var salt = RandomNumberGenerator.GetBytes(16);
        var pdb = new Rfc2898DeriveBytes(passphrase, salt, 10000, HashAlgorithmName.SHA256);
        aes.Key = pdb.GetBytes(32);
        aes.IV = pdb.GetBytes(16);

        using var ms = new MemoryStream();
        ms.Write(salt, 0, salt.Length);
        using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plaintext);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string encrypted, string? passphrase)
    {
        if (OperatingSystem.IsWindows())
        {
            var bytes = Convert.FromBase64String(encrypted);
            var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }

        if (string.IsNullOrEmpty(passphrase))
            throw new InvalidOperationException("Passphrase required for decryption on this platform.");

        var data = Convert.FromBase64String(encrypted);
        var salt = data.AsSpan(0, 16).ToArray();
        using var aes = Aes.Create();
        var pdb = new Rfc2898DeriveBytes(passphrase, salt, 10000, HashAlgorithmName.SHA256);
        aes.Key = pdb.GetBytes(32);
        aes.IV = pdb.GetBytes(16);
        using var ms = new MemoryStream(data, 16, data.Length - 16);
        using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }
}
