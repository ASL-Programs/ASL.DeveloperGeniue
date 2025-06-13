namespace DeveloperGeniue.Core;

public interface IConfigurationService
{
    Task<T?> GetSettingAsync<T>(string key);
    Task SetSettingAsync<T>(string key, T value);
}
