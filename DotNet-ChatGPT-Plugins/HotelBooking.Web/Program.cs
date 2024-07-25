using HotelBooking.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var serverApiBaseAddress = builder.Configuration.GetValue<string>("HotelBookingApi:BaseAddress");

builder.Services.AddHttpClient("HotelBookingApi", client => client.BaseAddress = new Uri(serverApiBaseAddress));

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("HotelBookingApi"));

await builder.Build().RunAsync();
