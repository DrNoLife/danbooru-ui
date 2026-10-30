using Danbooru.ApiWrapper.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Playwright;

namespace Danbooru.UI.E2ETests;

/// <summary>
/// Runs the real app on Kestrel with <see cref="FakeDanbooru"/> behind it, plus one Chromium for all tests.
/// </summary>
public sealed class AppFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory = new WebApplicationFactory<Program>()
        .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            services.Replace(ServiceDescriptor.Singleton<IDanbooruWrapper, FakeDanbooru>())));

    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = null!;
    public string BaseUrl { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        _factory.UseKestrel(0);
        _factory.StartServer();
        BaseUrl = _factory.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();

        // Downloads Chromium the first time; a no-op once it is there. CI installs it (with OS dependencies) beforehand.
        var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
        if (exitCode != 0)
        {
            throw new InvalidOperationException($"Installing Chromium for Playwright failed with exit code {exitCode}.");
        }

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync();
    }

    /// <summary>
    /// A fresh page (own cookies and storage) that has finished starting the interactive circuit.
    /// </summary>
    public async Task<IPage> OpenAsync(string path, int width = 1280, int height = 800)
    {
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = BaseUrl,
            ViewportSize = new ViewportSize { Width = width, Height = height },
            HasTouch = width < 700
        });

        var page = await context.NewPageAsync();
        await page.GotoAsync(path);
        await WaitUntilInteractiveAsync(page);
        return page;
    }

    public static Task WaitUntilInteractiveAsync(IPage page) =>
        page.WaitForFunctionAsync("() => document.documentElement.dataset.interactive !== undefined");

    public async ValueTask DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();
        await _factory.DisposeAsync();
    }
}
