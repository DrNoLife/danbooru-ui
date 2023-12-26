using Danbooru.UI.Client.Pages;
using Danbooru.UI.Components;
using Danbooru.ApiWrapper.Extensions;
using Danbooru.UI.Interfaces;
using Danbooru.UI.Services;
using Microsoft.AspNetCore.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents(options => options.DetailedErrors = true)
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDanbooruWrapper();

builder.Services.AddScoped<IGallerySettingsService, GallerySettingsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    //app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();
    //.AddAdditionalAssemblies(typeof(TagsSearch).Assembly);

app.Run();
