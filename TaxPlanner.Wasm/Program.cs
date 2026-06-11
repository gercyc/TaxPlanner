using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TaxPlanner.Wasm;
using TaxPlanner.Wasm.Services;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddHttpClient<BlogService>(client =>
{
    // Configure the API base URL. Change this to your Server URL.
    // For development: https://localhost:5001
    // For production: https://your-api-domain.com
    client.BaseAddress = new Uri("https://localhost:7145");
});
builder.Services.AddMudServices();
builder.Services.AddSingleton<CalculadoraIRService>();

await builder.Build().RunAsync();
