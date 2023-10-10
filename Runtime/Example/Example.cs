using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Weather;

public class Example : MonoBehaviour
{
    void Start()
    {
        var task = GetWeather();
    }

    private async Task GetWeather()
    {
        // You can disable component's logs by calling WeatherProvider.DisableLogs()

        var openMeteoService = new OpenMeteoService();
        var openWeatherAppId = "<openWeatherAppId>";
        var openWeatherService = new OpenWeatherMapService(openWeatherAppId)
        {
            MeasurementUnits = OpenWeatherMapService.EMeasurementUnits.Standard
        };
        var weatherApiAppKey = "<weatherApiAppKey>";
        var weatherApiService = new WeatherApiService(weatherApiAppKey)
        {
            SpeedType = WeatherApiService.ESpeedType.MPH,
            TemperatureType = WeatherApiService.ETemperatureType.Celsius
        };

        var provider = new WeatherProvider();

        // Adding services
        provider.AddService(openMeteoService);
        provider.AddService(openWeatherService);
        provider.AddService(weatherApiService);

        // Running requests for all services with location defined
        var token = new CancellationTokenSource();
        var result = await provider.GetWeather(52.52, 13.41, 5, token.Token);

        Debug.Log("RequestWithCoordinates--------------");
        Debug.Log("Result. OpenMeteoService -----------");
        Debug.Log(result.ResultList[0].ResultType);
        Debug.Log(result.ResultList[0].Error);
        Debug.Log(result.ResultList[0].Weather?.Temperature);
        Debug.Log(result.ResultList[0].Weather?.WindDirection);
        Debug.Log(result.ResultList[0].Weather?.WindSpeed);

        Debug.Log("Result. OpenWeatherMapService-------");
        Debug.Log(result.ResultList[1].ResultType);
        Debug.Log(result.ResultList[1].Error);
        Debug.Log(result.ResultList[1].Weather?.Temperature);
        Debug.Log(result.ResultList[1].Weather?.WindDirection);
        Debug.Log(result.ResultList[1].Weather?.WindSpeed);

        Debug.Log("Result. WeatherApiService-------");
        Debug.Log(result.ResultList[2].ResultType);
        Debug.Log(result.ResultList[2].Error);
        Debug.Log(result.ResultList[2].Weather?.Temperature);
        Debug.Log(result.ResultList[2].Weather?.WindDirection);
        Debug.Log(result.ResultList[2].Weather?.WindSpeed);

        // Running requests for all services with location undefined

        token = new CancellationTokenSource();

        result = await provider.GetWeather(5, token.Token);

        Debug.Log("RequestWithoutCoordinates-----------");
        Debug.Log("Result. OpenMeteoService -----------");
        Debug.Log(result.ResultList[0].ResultType);
        Debug.Log(result.ResultList[0].Error);
        Debug.Log(result.ResultList[0].Weather?.Temperature);
        Debug.Log(result.ResultList[0].Weather?.WindDirection);
        Debug.Log(result.ResultList[0].Weather?.WindSpeed);

        Debug.Log("Result. OpenWeatherMapService-------");
        Debug.Log(result.ResultList[1].ResultType);
        Debug.Log(result.ResultList[1].Error);
        Debug.Log(result.ResultList[1].Weather?.Temperature);
        Debug.Log(result.ResultList[1].Weather?.WindDirection);
        Debug.Log(result.ResultList[1].Weather?.WindSpeed);

        Debug.Log("Result. WeatherApiService-------");
        Debug.Log(result.ResultList[2].ResultType);
        Debug.Log(result.ResultList[2].Error);
        Debug.Log(result.ResultList[2].Weather?.Temperature);
        Debug.Log(result.ResultList[2].Weather?.WindDirection);
        Debug.Log(result.ResultList[2].Weather?.WindSpeed);

    }
}
