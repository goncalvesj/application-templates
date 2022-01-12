using GraphQL.MicrosoftDI;
using GraphQL.Server;
using GraphQL.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISchema, FilmSchema>(services => new FilmSchema(new SelfActivatingServiceProvider(services)));

builder.Services.AddGraphQL(options =>
{
    options.EnableMetrics = true;
}).AddSystemTextJson();

var app = builder.Build();

app.UseGraphQLAltair();

app.UseGraphQL<ISchema>();

app.UseHttpsRedirection();

app.Run();
