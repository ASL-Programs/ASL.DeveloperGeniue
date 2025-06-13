using System.Text.Json;

namespace DeveloperGeniue.Core;

public interface ILanguageService
{
    Task<string> GetStringAsync(string key, params object[] args);
    Task SetLanguageAsync(string languageCode);
    Task<string> GetUserLanguageAsync();
    string CurrentLanguage { get; }
}

public class LanguageService : ILanguageService
{
    private readonly IConfigurationService _config;
    private readonly string _resourcePath;
    private readonly Dictionary<string, Dictionary<string, string>> _cache = new();

    public LanguageService(IConfigurationService config, string? resourcePath = null)
    {
        _config = config;
        _resourcePath = resourcePath ?? Path.Combine(AppContext.BaseDirectory, "Resources");
        CurrentLanguage = _config.GetSettingAsync<string>("language").GetAwaiter().GetResult() ?? "en-US";
    }

    public string CurrentLanguage { get; private set; } = "en-US";

    public async Task SetLanguageAsync(string languageCode)
    {
        CurrentLanguage = languageCode;
        await _config.SetSettingAsync("language", languageCode);
    }

    public async Task<string> GetStringAsync(string key, params object[] args)
    {
        var lang = await _config.GetSettingAsync<string>("language") ?? CurrentLanguage;
        CurrentLanguage = lang;
        var resources = await LoadLanguageAsync(lang);
        if (!resources.TryGetValue(key, out var text))
        {
            if (lang != "en-US")
            {
                resources = await LoadLanguageAsync("en-US");
                if (resources.TryGetValue(key, out text))
                    return Format(text, args);
            }
            return key;
        }
        return Format(text, args);
    }

    public async Task<string> GetUserLanguageAsync()
    {
        var lang = await _config.GetSettingAsync<string>("language") ?? "en-US";
        CurrentLanguage = lang;
        return lang;
    }

    private static string Format(string text, object[] args) => args.Length > 0 ? string.Format(text, args) : text;

    private async Task<Dictionary<string, string>> LoadLanguageAsync(string lang)
    {
        if (_cache.TryGetValue(lang, out var cached))
            return cached;
        var file = Path.Combine(_resourcePath, $"{lang}.json");
        if (!File.Exists(file))
            return new Dictionary<string, string>();
        var json = await File.ReadAllTextAsync(file);
        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
        _cache[lang] = dict;
        return dict;
    }
}
