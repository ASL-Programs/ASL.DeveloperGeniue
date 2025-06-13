using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using DeveloperGeniue.Core;
using System.Runtime.Versioning;
#if WINDOWS
using System.Threading;
using System.Windows.Forms;
#endif

namespace DeveloperGeniue.CLI;

public static class HybridHost
{
    public static async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRazorPages();
        builder.Services.AddSingleton<IConfigurationService, DatabaseConfigurationService>();
        var app = builder.Build();
        app.MapRazorPages();
        app.MapGet("/", () => "DeveloperGeniue running");
        await app.StartAsync(cancellationToken);

#if WINDOWS
        var thread = new Thread(() => Application.Run(new Form { Text = "DeveloperGeniue" }));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
#endif
        await app.WaitForShutdownAsync(cancellationToken);
    }
}
