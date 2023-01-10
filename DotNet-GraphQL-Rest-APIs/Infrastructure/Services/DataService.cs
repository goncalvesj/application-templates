using System.Text.Json;

public class DataService
{
    private readonly string FilmDataLocation = File.Exists($"{AppContext.BaseDirectory}\\films.json")
        ? $"{AppContext.BaseDirectory}\\films.json"
        : "films.json";

    private readonly string PeopleDataLocation = File.Exists($"{AppContext.BaseDirectory}\\people.json")
        ? $"{AppContext.BaseDirectory}\\people.json"
        : "people.json";

    private readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public List<Film> GetFilms()
    {
        var jsonString = File.ReadAllText(FilmDataLocation);
        var films = JsonSerializer.Deserialize<List<Film>>(jsonString, options);

        return films;
    }

    public async Task<List<Film>> GetFilmsAsync()
    {
        using var openStream = File.OpenRead(FilmDataLocation);
        var films = await JsonSerializer.DeserializeAsync<List<Film>>(openStream, options);

        return films;
    }

    public async Task<List<People>> GetPeopleAsync()
    {
        using var openStream = File.OpenRead(PeopleDataLocation);
        var people = await JsonSerializer.DeserializeAsync<List<People>>(openStream, options);

        return people;
    }

    public async Task<List<Cast>> GetCastAsync()
    {
        var films = await GetFilmsAsync();
        var people = await GetPeopleAsync();

        var castList = films.Select(f => new Cast
        {
            Title = f.Fields.Title,
            Characters = f.Fields.Characters.Select(f => people.SingleOrDefault(p => p.Pk == f).Fields.Name).ToList()
        }).ToList();

        return castList;
    }
}