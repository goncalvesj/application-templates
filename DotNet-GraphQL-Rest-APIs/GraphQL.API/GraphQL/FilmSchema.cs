using GraphQL.Types;

public class FilmSchema : Schema
{
    public FilmSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Query = serviceProvider.GetRequiredService<FilmsQuery>();
    }
}