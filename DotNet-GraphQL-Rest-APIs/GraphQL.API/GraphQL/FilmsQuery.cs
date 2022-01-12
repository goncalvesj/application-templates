using GraphQL.Types;

public class FilmsQuery : ObjectGraphType
{
    public FilmsQuery()
    {      
        Name = "Queries";
        Description = "The base query for all the entities in our object graph.";

        Field<FilmType>("film",
            "Gets first film from file.",
            resolve: context =>
            {
                var data = new FilmService();
                var films = data.GetFilms();
                return films.FirstOrDefault();
            });

        FieldAsync<ListGraphType<FilmType>, List<Film>>("films",
            "Gets all films aync.",
           resolve: async context =>
           {
               var data = new FilmService();
               var films = await data.GetFilmsAsync();
               return films;
           });
    }
}
