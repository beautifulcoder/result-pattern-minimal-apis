using Microsoft.EntityFrameworkCore;

namespace ResultPattern;

public sealed class WeatherDbContext : DbContext
{
  public DbSet<WeatherDto> WeatherDb { get; set; }
  public DbSet<CityDto> CityDb { get; set; }

  public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options)
  {
    Database.EnsureCreated();
    SeedDatabase();
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseInMemoryDatabase("WeatherDatabase");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<CityDto>().HasKey(c => c.Code);
    modelBuilder.Entity<WeatherDto>().HasKey(w => new {w.CityCode, w.Hour});
  }

  private void SeedDatabase()
  {
    if (CityDb.Any())
    {
      return;
    }

    List<CityDto> cities =
    [
      new("NY", "New York"),
      new("LA", "Los Angeles"),
      new("CH", "Chicago"),
      new("HO", "Houston"),
      new("PH", "Phoenix"),
      new("SA", "San Antonio"),
      new("SD", "San Diego"),
      new("DA", "Dallas")
    ];

    foreach (var city in cities)
    {
      CityDb.Add(city);
    }

    var conditions = new List<string> { "Sunny", "Rainy", "Cloudy", "Snowy", "Windy" };
    var unpredictableWeather = new Random();

    foreach (var city in cities)
    {
      for (var i = 0; i < 24; i++)
      {
        WeatherDb.Add(new WeatherDto(
          city.Code,
          i,
          unpredictableWeather.Next(0, 100),
          conditions[unpredictableWeather.Next(0, 4)],
          unpredictableWeather.Next(0, 100))
        );
      }
    }

    SaveChanges();
  }
}
