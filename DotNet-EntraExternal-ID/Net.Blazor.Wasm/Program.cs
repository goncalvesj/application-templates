using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Net.Blazor.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";
});

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

var serverApiBaseAddress = builder.Configuration.GetValue<string>("WeatherApi:BaseAddress");

builder.Services.AddHttpClient("WeatherApi", client => client.BaseAddress = new Uri(serverApiBaseAddress))
              .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("WeatherApi.Unauthenticated", client => client.BaseAddress = new Uri(serverApiBaseAddress));

builder.Services.AddTransient(sp =>
sp.GetRequiredService<IHttpClientFactory>().CreateClient("WeatherApi"));

builder.Services.AddTransient(sp =>
sp.GetRequiredService<IHttpClientFactory>().CreateClient("WeatherApi.Unauthenticated"));

await builder.Build().RunAsync();

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager, IConfiguration configuration) : base(provider, navigationManager)
    {
        var scopes = configuration.GetSection("WeatherApi:Scopes").Get<string[]>();
        var baseAddress = configuration["WeatherApi:BaseAddress"];

        ConfigureHandler(
           authorizedUrls: new[] { baseAddress },
           scopes: scopes
           );
    }
}
