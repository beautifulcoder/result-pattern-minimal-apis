namespace ResultPattern;

public class CrummyWeatherService(WeatherDbContext dbContext, ILogger<WeatherService> logger)
{
  private readonly WeatherDbContext _dbContext = dbContext;
  private readonly ILogger<WeatherService> _logger = logger;

  public WeatherModel GetWeather(string cityCode)
  {
    if (cityCode.Length != 2 || cityCode.Any(char.IsDigit))
    {
      throw new InvalidException("City code must be three non-digit characters");
    }

    try
    {
      if (cityCode == "HO")
      {
        throw new InternalServerErrorException("An error occurred");
      }

      var city = _dbContext.CityDb.FirstOrDefault(c => c.Code == cityCode);

      if (city is null)
      {
        throw new NotFoundException($"City with code {cityCode} not found");
      }

      var weather = _dbContext.WeatherDb.Where(w => w.CityCode == city.Code).ToList();

      return new WeatherModel(
        new CityModel(
          city.Code,
          city.Name),
        weather.Select(w => new HourlyModel(
          w.Hour,
          w.Temperature,
          w.Condition,
          w.Humidity)).ToList()
      );
    }
    catch (InternalServerErrorException ex)
    {
      _logger.LogError(ex, "An error occurred");
      throw new InternalServerErrorException(ex.Message);
    }
  }
}

public class InvalidException(string _message) : Exception(_message);
public class NotFoundException(string _message) : Exception(_message);
public class InternalServerErrorException(string _message) : Exception(_message);
