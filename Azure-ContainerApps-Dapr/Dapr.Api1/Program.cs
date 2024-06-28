using System.Text.Json.Serialization;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

// Set up configuration sources.

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("api/hello", () =>
{
    return "Hello From App 1!";
})
.WithOpenApi();

app.MapGet("api/secret-store", async () =>
{
    var SECRET_STORE_NAME = configuration.GetValue<string>("DAPR_SECRET_STORE");

    using var client = new DaprClientBuilder().Build();

    //Using Dapr SDK to get a secret
    var secret = await client.GetSecretAsync(SECRET_STORE_NAME, "MySecret");

    return $"Result: {string.Join(", ", secret)}";
})
.WithOpenApi();

app.MapPost("api/service-to-service", async () =>
{
    using var client = new DaprClientBuilder().Build();

    var consumerId = configuration.GetValue<string>("DAPR_CONSUMER_ID");
    var consumerMethod = configuration.GetValue<string>("DAPR_CONSUMER_METHOD");

    for (int i = 1; i <= 20; i++)
    {

        var order = new Order(i);

        // Invoking a service
        await client.InvokeMethodAsync(consumerId, consumerMethod, order);

        Console.WriteLine("Order send : " + order);
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return "Orders sent!";
})
.WithOpenApi();

// app.MapPost("app1/publish", async () =>
// {

//     var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500"); //reconfigure cpde to make requests to Dapr sidecar
//     const string PUBSUBNAME = "pubsub";
//     const string TOPIC = "orders";
//     Console.WriteLine($"Publishing to baseURL: {baseURL}, Pubsub Name: {PUBSUBNAME}, Topic: {TOPIC} ");

//     var httpClient = new HttpClient();
//     httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

//     for (int i = 1; i <= 10; i++)
//     {
//         var order = new Order(i);
//         var orderJson = JsonSerializer.Serialize<Order>(order);
//         var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

//         // Publish an event/message using Dapr PubSub via HTTP Post
//         var response = httpClient.PostAsync($"{baseURL}/v1.0/publish/{PUBSUBNAME}/{TOPIC}", content);
//         Console.WriteLine("Published data: " + order);

//         await Task.Delay(TimeSpan.FromSeconds(1));
//     }

//     return "Orders sent using http!";
// })
// .WithOpenApi();

app.MapPost("api/pub-sub", async () =>
{
    using var client = new DaprClientBuilder().Build();

    var topic = configuration.GetValue<string>("DAPR_TOPIC");

    for (int i = 1; i <= 10; i++)
    {
        var order = new Order(i);

        // Publish an event/message using Dapr PubSub
        await client.PublishEventAsync("pubsub", topic, order);
        Console.WriteLine("Published data: " + order);

        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return "Orders sent!";
})
.WithOpenApi();

app.Run();

public record Order([property: JsonPropertyName("orderId")] int OrderId);

// public class AppInsightsTelemetryInitializer : ITelemetryInitializer
// {
//     public void Initialize(ITelemetry telemetry)
//     {
//         if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
//         {
//             //set custom role name here
//             telemetry.Context.Cloud.RoleName = "tasksmanager-backend-api";
//         }
//     }
// }