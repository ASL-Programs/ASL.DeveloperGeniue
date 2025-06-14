using DeveloperGeniue.Core;
using System.IO;
using System.Text.Json;
using Xunit;

namespace DeveloperGeniue.Tests;

public class ApiKeyEncryptionTests
{
    [Fact]
    public async Task FileServiceEncryptsApiKeys()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var service = new ConfigurationService(path, "pass");
        await service.SetSettingAsync("MyServiceApiKey", "secret");
        var json = await File.ReadAllTextAsync(path);
        Assert.DoesNotContain("secret", json);
        var decrypted = CryptoHelper.Decrypt(await service.GetSettingAsync<string>("MyServiceApiKey")!, "pass");
        Assert.Equal("secret", decrypted);
        File.Delete(path);
    }

    [Fact]
    public async Task DatabaseServiceEncryptsApiKeys()
    {
        var db = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var service = new DatabaseConfigurationService(db, "pass");
        await service.SetSettingAsync("MyServiceApiKey", "secret");
        service = new DatabaseConfigurationService(db, "pass");
        var val = await service.GetSettingAsync<string>("MyServiceApiKey");
        Assert.NotEqual("secret", val);
        var decrypted = CryptoHelper.Decrypt(val!, "pass");
        Assert.Equal("secret", decrypted);
        File.Delete(db);
    }
}
