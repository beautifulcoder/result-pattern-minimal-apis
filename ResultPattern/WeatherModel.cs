namespace ResultPattern;

public record WeatherModel(
  CityModel City,
  List<HourlyModel> Weather);

public record CityModel(
  string Code,
  string Name);

public record HourlyModel(
  int Hour,
  int Temperature,
  string Condition,
  int Humidity);
