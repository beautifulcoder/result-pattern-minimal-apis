namespace ResultPattern;

public class WeatherService(WeatherDbContext dbContext, ILogger<WeatherService> logger)
{
  private readonly WeatherDbContext _dbContext = dbContext;
  private readonly ILogger<WeatherService> _logger = logger;

  public Result<WeatherModel> GetWeather(string cityCode)
  {
    if (cityCode.Length != 2 || cityCode.Any(char.IsDigit))
    {
      return Result<WeatherModel>.Invalid("City code must be three non-digit characters");
    }

    try
    {
      if (cityCode == "HO")
      {
        throw new Exception("An error occurred");
      }

      var city = _dbContext.CityDb.FirstOrDefault(c => c.Code == cityCode);

      if (city is null)
      {
        return Result<WeatherModel>.NotFound($"City with code {cityCode} not found");
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
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred");
      return Result<WeatherModel>.InternalServerError(ex.Message);
    }
  }
}
