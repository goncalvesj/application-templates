using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/people", async () =>
{
    var data = new DataService();
    var films = await data.GetPeopleAsync();
    return films;
});

app.Run();