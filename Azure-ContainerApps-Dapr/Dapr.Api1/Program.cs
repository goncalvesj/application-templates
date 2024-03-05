using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using Dapr.Client;
using System.Security.Principal;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }


app.UseHttpsRedirection();

app.MapGet("app1/hello", () =>
{
    return "Hello From App 1!";
})
.WithOpenApi();

app.MapPost("app1/order", async () =>
{

    var client = new DaprClientBuilder().Build();

    for (int i = 1; i <= 20; i++)
    {

        var order = new Order(i);

        // Invoking a service
        await client.InvokeMethodAsync("dapr-app-2", "app2/order", order);

        Console.WriteLine("Order send : " + order);
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return "Orders sent!";
})
.WithOpenApi();

app.MapPost("app1/publish", async () =>
{

    var baseURL = (Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost") + ":" + (Environment.GetEnvironmentVariable("DAPR_HTTP_PORT") ?? "3500"); //reconfigure cpde to make requests to Dapr sidecar
    const string PUBSUBNAME = "pubsub";
    const string TOPIC = "orders";
    Console.WriteLine($"Publishing to baseURL: {baseURL}, Pubsub Name: {PUBSUBNAME}, Topic: {TOPIC} ");

    var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

    for (int i = 1; i <= 10; i++)
    {
        var order = new Order(i);
        var orderJson = JsonSerializer.Serialize<Order>(order);
        var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

        // Publish an event/message using Dapr PubSub via HTTP Post
        var response = httpClient.PostAsync($"{baseURL}/v1.0/publish/{PUBSUBNAME}/{TOPIC}", content);
        Console.WriteLine("Published data: " + order);

        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return "Orders sent using http!";
})
.WithOpenApi();

app.MapPost("app1/publishsdk", async () =>
{

    using var client = new DaprClientBuilder().Build();

    for (int i = 1; i <= 10; i++)
    {
        var order = new Order(i);

        // Publish an event/message using Dapr PubSub
        await client.PublishEventAsync("pubsub", "orders", order);
        Console.WriteLine("Published data: " + order);

        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return "Orders sent using sdk!";
})
.WithOpenApi();

app.Run();

public record Order([property: JsonPropertyName("orderId")] int OrderId);