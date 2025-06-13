using DeveloperGeniue.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeveloperGeniue.CLI;

public class SettingsModel : PageModel
{
    private readonly IConfigurationService _config;
    public string Language { get; set; } = "en-US";

    public SettingsModel(IConfigurationService config)
    {
        _config = config;
    }

    public async Task OnGetAsync()
    {
        Language = await _config.GetSettingAsync<string>("language") ?? "en-US";
    }

    public async Task OnPostAsync()
    {
        if (Request.Form.TryGetValue("language", out var lang))
        {
            await _config.SetSettingAsync("language", lang.ToString());
            Language = lang;
        }
    }
}
