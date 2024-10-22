namespace ResultPattern;

public record WeatherDto(
  string CityCode,
  int Hour,
  int Temperature,
  string Condition,
  int Humidity)
{
  public WeatherDto() : this("", 0, 0, "", 0) {}
}

public record CityDto(
  string Code,
  string Name)
{
  public CityDto() : this("", "") {}
}
