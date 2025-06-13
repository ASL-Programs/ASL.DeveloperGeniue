using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class ConfigurationServiceTests
{
    [Fact]
    public async Task SetAndGetSetting()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var service = new ConfigurationService(tempFile);

        await service.SetSettingAsync("TestKey", "Value");
        var result = await service.GetSettingAsync<string>("TestKey");

        Assert.Equal("Value", result);
        File.Delete(tempFile);
    }
}
