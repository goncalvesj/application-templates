using GraphQL.Types;

public class FilmDataType : ObjectGraphType<Fields>
{
    public FilmDataType()
    {
        Name = "FilmData";
        Description = "FilmData Type";
        Field(d => d.Characters, nullable: true).Description("");
        Field(d => d.Created, nullable: true).Description("");
        Field(d => d.Director, nullable: true).Description("");
        Field(d => d.Edited, nullable: true).Description("");
        Field(d => d.Episode_id, nullable: true).Description("");
        Field(d => d.Opening_crawl, nullable: true).Description("");
        Field(d => d.Planets, nullable: true).Description("");
        Field(d => d.Producer, nullable: true).Description("");
        Field(d => d.Release_date, nullable: true).Description("");
        Field(d => d.Species, nullable: true).Description("");
        Field(d => d.Starships, nullable: true).Description("");
        Field(d => d.Title, nullable: true).Description("");
        Field(d => d.Vehicles, nullable: true).Description("");
    }
}