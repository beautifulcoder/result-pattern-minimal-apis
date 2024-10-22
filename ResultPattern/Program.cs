using ResultPattern;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WeatherDbContext>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<CrummyWeatherService>();

var app = builder.Build();

app.MapGet("/crummyWeather/cities/{cityCode}", (CrummyWeatherService service, string cityCode) =>
{
  try
  {
    return Results.Ok(service.GetWeather(cityCode));
  }
  catch (InvalidException ex)
  {
    return Results.BadRequest(ex.Message);
  }
  catch (NotFoundException ex)
  {
    return Results.NotFound(ex.Message);
  }
  catch (InternalServerErrorException)
  {
    return Results.StatusCode(500);
  }
  catch (Exception ex)
  {
    return Results.Problem(ex.Message);
  }
});

app.MapGet("/fairWeather/cities/{cityCode}", (WeatherService service, string cityCode) =>
{
  var result = service.GetWeather(cityCode);

  if (result.IsSuccess)
  {
    return Results.Ok(result.Value);
  }

  if (result.Error.ErrorType == ErrorType.NotFound)
  {
    return Results.NotFound(result.Error.Error);
  }

  if (result.Error.ErrorType == ErrorType.Invalid)
  {
    return Results.BadRequest(result.Error.Error);
  }

  if (result.Error.ErrorType == ErrorType.InternalServerError)
  {
    return Results.StatusCode(500);
  }

  return Results.Problem(result.Error.Error);
});

app.MapGet("/weather/cities/{cityCode}", (WeatherService service, string cityCode) =>
  service.GetWeather(cityCode).Match(
    ok: Results.Ok,
    error: error => error.ErrorType switch
    {
      ErrorType.NotFound => Results.NotFound(error.Error),
      ErrorType.Invalid => Results.BadRequest(error.Error),
      ErrorType.InternalServerError => Results.StatusCode(500),
      _ => Results.Problem(error.Error)
    }
  )
);

app.Run();
