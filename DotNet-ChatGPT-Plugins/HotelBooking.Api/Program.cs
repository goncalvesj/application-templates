using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planners;
using Plugins.PluginShared;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Booking",
        Version = "v1",
        Description = "A plugin that allows you to search and book hotels"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://chat.openai.com", "http://localhost:3000", "http://localhost:3001", "https://localhost:7239").AllowAnyHeader().AllowAnyMethod();
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
    x.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Hotel.Booker v1");

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
        NameForModel = "hotelbooker",
        NameForHuman = "Hotel Booker",
        DescriptionForModel = "Plugin for Hotel Bookings, Get Hotels, Book hotel, Cancel bookings, etc.",
        DescriptionForHuman = "Manages hotel bookings, advice hotels, book a hotel or room, etc.",
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

app.MapGet("/GetHotels", () =>
{
    return HotelData.GetHotels();
})
.WithName("GetHotels")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Get the list of hotels";
    return generatedOperation;
});

app.MapPost("/GetHotelsWithSK", async (PromptData promptData) =>
{
    var text = promptData.Text;
    var result = new List<Hotel>();

    if (!string.IsNullOrWhiteSpace(text))
    {
        var builder = new KernelBuilder()
        .WithAzureOpenAIChatCompletionService(
                 Environment.GetEnvironmentVariable("aoai-deployment-name"),
                 Environment.GetEnvironmentVariable("aoai-aoai-endpoint"),
                 Environment.GetEnvironmentVariable("aoai-key"));        

        var kernel = builder.Build();

        var ask = $@"
System: You summarize the users question into in a single sentence.

User: {text}";

        //User: I need to relax, what hotels do you have without Wifi?";

        kernel.ImportFunctions(new HotelFilterPlugin(HotelData.GetHotels()), "HotelFilter");

        var planner = new SequentialPlanner(kernel);

        var plan = await planner.CreatePlanAsync(ask);

        var kernelResult = await kernel.RunAsync(plan);

        var data = kernelResult.GetValue<string>();
        result = JsonSerializer.Deserialize<List<Hotel>>(data);

        return result;
    }

    return HotelData.GetHotels();
})
.ExcludeFromDescription();
//.WithName("GetHotelsWithSK")
//.WithOpenApi(generatedOperation =>
//{
//    generatedOperation.Description = "Get the list of hotels filteres by a LLM";
//    return generatedOperation;
//});

app.MapPost("/BookHotel", (Booking booking) =>
{
    return $"Hotel booked successfully. Reservation Id: {Guid.NewGuid()}";
})
.WithName("BookHotel")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Performs an Hotel booking. Requires the hotel name and the number of days to book for.";
    return generatedOperation;
});

app.MapPost("/CancelBooking", (CancelBooking cancelBooking) =>
{
    return $"Booking cancelled successfully. ID: {cancelBooking.ReservationId}";
})
.WithName("CancelBooking")
.WithOpenApi(generatedOperation =>
{
    generatedOperation.Description = "Cancel an Hotel Booking. Requires the reservation id.";
    return generatedOperation;
});

app.Run();
