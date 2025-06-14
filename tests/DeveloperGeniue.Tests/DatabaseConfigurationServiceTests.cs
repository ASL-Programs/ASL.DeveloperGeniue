using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class DatabaseConfigurationServiceTests
{
    [Fact]
    public async Task SetAndGetSetting_PersistsValue()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var service = new DatabaseConfigurationService(dbPath);

        await service.SetSettingAsync("TestKey", "TestValue");

        service = new DatabaseConfigurationService(dbPath);
        var result = await service.GetSettingAsync<string>("TestKey");

        Assert.Equal("TestValue", result);
        File.Delete(dbPath);
    }
}
