using DeveloperGeniue.Core;

namespace DeveloperGeniue.Tests;

public class LanguageServiceLocalizationTests
{
    [Theory]
    [InlineData("tr-TR", "Komutlar:", "Kaydet")]
    [InlineData("ru-RU", "Команды:", "Сохранить")]
    public async Task LoadsLocalizedStrings(string language, string cliExpected, string uiExpected)
    {
        var configFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var config = new ConfigurationService(configFile);
        var service = new LanguageService(config, Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Resources"));
        await service.SetLanguageAsync(language);
        var result = await service.GetStringAsync("CLI.Commands");
        Assert.Equal(cliExpected, result);
        var ui = await service.GetStringAsync("UI.Save");
        Assert.Equal(uiExpected, ui);
        File.Delete(configFile);
    }
}
