using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PromoHunter.WebUI;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurando URL base para o backend da API minimal (por default usaremos localhost 5025 ou mudamos pra porta q for exigida)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5025") });
builder.Services.AddMudServices();

await builder.Build().RunAsync();
