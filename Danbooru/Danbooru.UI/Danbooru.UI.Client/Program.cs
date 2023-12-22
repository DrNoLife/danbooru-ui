using Danbooru.ApiWrapper.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddDanbooruWrapper();

await builder.Build().RunAsync();
