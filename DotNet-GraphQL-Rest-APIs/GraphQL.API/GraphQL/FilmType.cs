using GraphQL.Types;

public class FilmType : ObjectGraphType<Film>
{
    public FilmType()
    {
        Name = "Film";
        Description = "Film Type";
        Field(d => d.Model, nullable: true).Description("Resource Type");
        Field(d => d.Pk, nullable: true).Description("Film Id");
        Field(
          name: "Fields",
          description: "Data of the movie",
          type: typeof(FilmDataType),
          resolve: m => m.Source.Fields);       
    }
}