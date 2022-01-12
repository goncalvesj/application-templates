using System.Text.Json;

public class FilmService
{
    private readonly string FilmDataLocation = File.Exists($"{AppContext.BaseDirectory}\\films.json") 
        ? $"{AppContext.BaseDirectory}\\films.json" 
        : "films.json";

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
}