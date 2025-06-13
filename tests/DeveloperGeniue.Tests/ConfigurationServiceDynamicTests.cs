using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class ConfigurationServiceDynamicTests
{
    [Fact]
    public async Task GetAllSettingsReturnsValues()
    {
        var file = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var service = new ConfigurationService(file);
        await service.SetSettingAsync("A", 1);
        var all = await service.GetAllSettingsAsync();
        File.Delete(file);
        Assert.True(all.ContainsKey("A"));
    }
}
