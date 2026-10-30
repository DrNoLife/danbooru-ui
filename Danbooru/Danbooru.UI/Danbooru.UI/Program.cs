using Danbooru.ApiWrapper.Extensions;
using Danbooru.ApiWrapper.Interfaces;
using Danbooru.UI.Components;
using Danbooru.UI.Services;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(options => options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents()
    // The browser sends prerendered state (a page of posts, ~120 KB) back when the circuit starts; the 32 KB default is too small.
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 512 * 1024);

builder.Services.AddDanbooruWrapper();

// Downloads stream whole files, which can take longer than the API client's resilience timeouts allow.
builder.Services.AddHttpClient(DownloadClientName, client =>
{
    client.Timeout = TimeSpan.FromMinutes(10);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Imeji/1.0 (+https://github.com/DrNoLife/danbooru-ui)");
});

builder.Services.AddScoped<ImejiJs>();
builder.Services.AddScoped<Preferences>();
builder.Services.AddScoped<SearchSession>();
builder.Services.AddScoped<KeyboardShortcuts>();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapHealthChecks("/healthz");
// Lets a deploy check which release is running.
app.MapGet("/version", () => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

// Saves a post's original file under a readable name. Browsers ignore the download attribute on cross-origin links,
// so the file is streamed through here.
app.MapGet("/download/{id:int}", async (int id, IDanbooruWrapper danbooru, IHttpClientFactory httpClientFactory, HttpContext context) =>
{
    try
    {
        var post = await danbooru.Posts.GetPostByIdAsync(id);

        // Only ever fetch from Danbooru's own servers.
        if (!Uri.TryCreate(post.FileUrl, UriKind.Absolute, out var fileUri)
            || fileUri.Scheme != Uri.UriSchemeHttps
            || !(fileUri.Host == "donmai.us" || fileUri.Host.EndsWith(".donmai.us", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.NotFound();
        }

        var response = await httpClientFactory.CreateClient(DownloadClientName).GetAsync(fileUri, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.RegisterForDispose(response);
        if (!response.IsSuccessStatusCode)
        {
            return Results.StatusCode(StatusCodes.Status502BadGateway);
        }

        var stream = await response.Content.ReadAsStreamAsync(context.RequestAborted);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        return Results.Stream(stream, contentType, $"danbooru_{post.Id}.{post.FileExt}");
    }
    catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
        return Results.NotFound();
    }
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program
{
    private const string DownloadClientName = "danbooru-files";
}
