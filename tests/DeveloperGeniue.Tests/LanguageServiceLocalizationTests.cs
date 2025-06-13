using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class LanguageServiceLocalizationTests
{
    [Theory]
    [InlineData("tr-TR", "Komutlar:")]
    [InlineData("ru-RU", "Команды:")]
    public async Task LoadsLocalizedStrings(string language, string expected)
    {
        var configFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(configFile);
        var service = new LanguageService(config, Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Resources"));
        await service.SetLanguageAsync(language);
        var result = await service.GetStringAsync("CLI.Commands");
        Assert.Equal(expected, result);
        File.Delete(configFile);
    }
}
