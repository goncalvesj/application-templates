using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;


public class HotelFilterPlugin
{
    private List<Hotel> hotels;

    public HotelFilterPlugin(List<Hotel> hotels)
    {
        this.hotels = new List<Hotel>(hotels);
    }
  
    [SKFunction, Description("Filters the list of hotels by some arguments")]
    public string FilterHotels(
        [Description("The name of the hotel"), SKName("name")] string name,
        [Description("The country or location of the hotel"), SKName("country")] string country,
        [Description("If the hotel Wifi"), SKName("hasWifi")] string hasWifi,
        [Description("The minimum number of stars the hotel has"), SKName("minStars")] string minStars,
        [Description("The maximum number of stars the hotel has"), SKName("maxStars")] string maxStars)
    {
        List<Hotel> filteredHotels = hotels;

        if (!string.IsNullOrEmpty(name))
        {
            Console.WriteLine("name:" + name);
            filteredHotels = filteredHotels.Where(hotel => hotel.Name.Contains(name)).ToList();
        }

        if (!string.IsNullOrEmpty(country))
        {
            Console.WriteLine("country:" + country);
            filteredHotels = filteredHotels.Where(hotel => hotel.Address.Contains(country)).ToList();
        }

        if (!string.IsNullOrEmpty(minStars))
        {
            Console.WriteLine("min_stars:" + minStars);
            filteredHotels = filteredHotels.Where(hotel => hotel.Stars >= int.Parse(minStars)).ToList();
        }

        if (!string.IsNullOrEmpty(maxStars))
        {
            Console.WriteLine("max_stars:" + maxStars);
            filteredHotels = filteredHotels.Where(hotel => hotel.Stars <= int.Parse(maxStars)).ToList();
        }
        if (!string.IsNullOrEmpty(hasWifi))
        {
            var hasWifiBool = bool.Parse(hasWifi);
            filteredHotels = filteredHotels.Where(hotel => hotel.HasWifi ==hasWifiBool).ToList();
        }

        var json = JsonSerializer.Serialize(filteredHotels);

        return json;
    }
}


public static class HotelData
{
    public static List<Hotel> GetHotels()
    {
        return new List<Hotel>
     {
        new() {
            Address = "Mercè Rodoreda, 7, 17300, Blanes, Spain",
            Name = "Beverly Park Hotel & Spa Blanes",
            Description = "Located in Blanes in Costa Brava, Beverly Park Hotel & Spa Blanes is a four-star family hotel with a gym and swimming pool.",
            Country = "Spain",
            Stars = 4,
            Beds = 2,
            Bathrooms = 1,
            HasWifi = true,
            Price = 90,
            Currency = "EUR"
            },
         new() {
            Address = "Rue de la Brasserie 11, 1050 Ixelles, Belgium",
            Name = "Brussels Marriott Hotel Grand Place",
            Description = "Located in Belgium, this is a four-star family hotel with a cinema.",
            Country = "Belgium",
            Stars = 4,
            Beds = 2,
            Bathrooms = 1,
            HasWifi = true,
            Price = 90,
            Currency = "EUR"
            },
        new() {
            Address = "Carrer de la Riera, 77, 17300, Blanes, Spain",
            Name = "Hotel Blaumar Blanes",
            Description = "Located in Blanes in Costa Brava, Hotel Blaumar Blanes is a 2-star family hotel.",
            Country = "Spain",
            Stars = 2,
            Beds = 1,
            Bathrooms = 1,
            HasWifi = false,
            Price = 50,
            Currency = "EUR"
            },
        new() {
            Address = "Carrer de la Riera, 17300, Blanes, Spain",
            Name = "Hotel Costa Brava Blanes",
            Description = "Located in Blanes in Costa Brava, Hotel Costa Brava Blanes is a five-star family hotel with a gym and swimming pool.",
            Country = "Spain",
            Stars = 5,
            Beds = 4,
            Bathrooms = 2,
            HasWifi = true,
            Price = 190,
            Currency = "EUR"
            },
    };
    }
}