using Danbooru.UI.Components;
using Danbooru.ApiWrapper.Extensions;
using Danbooru.UI.Interfaces;
using Danbooru.UI.Services;
using Danbooru.UI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents(options => options.DetailedErrors = true)
    .AddInteractiveServerComponents();

builder.Services.Configure<DanbooruSettings>(options =>
    builder.Configuration.GetSection("DanbooruSettings").Bind(options));

builder.Services.AddDanbooruWrapper();

builder.Services.AddScoped<IGallerySettingsService, GallerySettingsService>();
builder.Services.AddScoped<IDoomScrollService, DoomScrollService>();
builder.Services.AddScoped<IMediaDownloaderService, MediaDownloaderService>();

var app = builder.Build();

app.UseExceptionHandler("/Error", createScopeForErrors: true);
app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
