using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Plugins.PluginShared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather.Forecast v1",
        Version = "v1",
        Description = "A plugin that allows you to get current weather information for a given city"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:3000", "http://localhost:3001").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
    });
});
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Weather.Forecast v1");

});

app.UseCors("AllowAll");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), ".well-known")),
    RequestPath = "/.well-known"
});

app.UseHttpsRedirection();

app.MapGet("/.well-known/ai-plugin.json", (HttpContext context) =>
{
    var pluginManifest = new PluginManifest()
    {
        NameForModel = "weather",
        NameForHuman = "Weather Plugin",
        DescriptionForModel = "Plugin for getting current weather information for a given city name.",
        DescriptionForHuman = "Plugin for getting current weather information for a given city name.",
        Auth = new PluginAuth()
        {
            Type = "none"
        },
        Api = new PluginApi()
        {
            Type = "openapi",
            Url = $"{context.Request.Scheme}://{context.Request.Host}/swagger/v1/swagger.yaml"
        },
        LogoUrl = $"{context.Request.Scheme}://{context.Request.Host}/.well-known/logo.png",
    };

    return pluginManifest;
})
.ExcludeFromDescription();

/// <summary>
/// Gets the weather for a latitude and longitude
/// </summary>
app.MapGet("/weather", async (double latitude, double longitude) =>
{
    using (var httpClient = new HttpClient())
    {
        var queryParams = $"latitude={latitude}&longitude={longitude}&hourly=temperature_2m&current_weather=true";
        string OPEN_MATEO_BASEURL = "https://api.open-meteo.com/v1/forecast?";
        var url = OPEN_MATEO_BASEURL + queryParams;
        var result = await httpClient.GetStringAsync(url);
        var jsonDocument = JsonDocument.Parse(result);
        var currentWeatherElement = jsonDocument.RootElement.GetProperty("current_weather");
        return JsonSerializer.Deserialize<GetWeatherResponse>(currentWeatherElement);

    }
})
.WithName("GetWeatherForecast")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Gets the current weather for a city by latitude and longitude";
    var parameter = generatedOperation.Parameters[0];
    parameter.Description = "The latitude of the city to retrieve the current weather";
    parameter = generatedOperation.Parameters[1];
    parameter.Description = "The longitude of the city to retrieve the current weather";

    return generatedOperation;
});

app.Run();

internal class GetWeatherResponse
{
    public double temperature { get; set; }
    public double windspeed { get; set; }
    public double winddirection { get; set; }
    public GetWeatherResponse() { }
    public GetWeatherResponse(double temperature, double windspeed, double winddirection)
    {
        this.temperature = temperature;
        this.windspeed = windspeed;
        this.winddirection = winddirection;
    }
}
