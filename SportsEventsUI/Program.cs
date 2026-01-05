using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SportsEventsUI;
// [CRITICAL] These are needed to stop the "Loading..." crash
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using SportsEventsUI.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// [CRITICAL] Connects to the new Backend Port (8000)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:8000") });

// [CRITICAL] These 3 lines fix the "Stuck at Loading" error
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();