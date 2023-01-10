var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/people", async () =>
{
    var data = new DataService();
    var people = await data.GetPeopleAsync();
    return people;
});

app.MapGet("/films", async () =>
{
    var data = new DataService();
    var films = await data.GetFilmsAsync();
    return films;
});

app.MapGet("/cast", async () =>
{
    var data = new DataService();
    var castList = await data.GetCastAsync();

    return castList;
});

app.Run();