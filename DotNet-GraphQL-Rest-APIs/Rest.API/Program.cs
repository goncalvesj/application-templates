using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/film", async () =>
{
    var data = new FilmService();
    var films = await data.GetFilmsAsync();
    return films.FirstOrDefault();
});

app.MapGet("/films", async () =>
{
    var data = new FilmService();
    var films = await data.GetFilmsAsync();
    return films;
});

app.Run();