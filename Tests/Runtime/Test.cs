using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Weather;

public class Test : MonoBehaviour
{
    void Start()
    {
        var openMeteoService = new OpenMeteoService();
        var openWeatherAppId = "2799cfd68f8fe65094d117c43225d5d0";
        var openWeatherService = new OpenWeatherMapService(openWeatherAppId);
        var weatherApiAppKey = "79f71fadd4ae418e868183707230810";
        var weatherApiService = new WeatherApiService(weatherApiAppKey);

        var provider = new WeatherProvider();
        provider.AddService(openMeteoService);
        provider.AddService(openWeatherService);
        provider.AddService(weatherApiService);

        var task = RunTest(provider);
    }

    private async Task RunTest(WeatherProvider provider)
    {
        await RequestWithCoordinates(provider);
        await RequestWithoutCoordinates(provider);
    }

    private async Task RequestWithCoordinates(WeatherProvider provider)
    {
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
    }

    private async Task RequestWithoutCoordinates(WeatherProvider provider)
    {
        var token = new CancellationTokenSource();

        var result = await provider.GetWeather(5, token.Token);

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
