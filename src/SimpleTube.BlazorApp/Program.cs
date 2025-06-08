using BlazorComponentBus;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleTube.BlazorApp;
using SimpleTube.BlazorApp.Infrastructure;
using SimpleTube.BlazorApp.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSingleton<IComponentBus, ComponentBus>();

builder.Services.AddInfrastructure();
builder.Services.AddSingleton(
    new ServerUrlProvider { ServerUrl = builder.Configuration.GetConnectionString("Server")! }
);

await builder.Build().RunAsync();
