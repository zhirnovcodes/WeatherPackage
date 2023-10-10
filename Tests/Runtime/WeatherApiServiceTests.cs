using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;
using Weather;
using System.Threading;
using System;

[TestFixture]
public class WeatherApiServiceTests
{
    [Test]
    public void GetWeather_SuccessWithContentFromFile()
    {
        // Arrange
        var testTextFile = (TextAsset)Resources.Load("Test/WeatherApi");
        var successJson = testTextFile.text;
        var httpRequestManager = Substitute.For<IHttpRequestManager>();
        httpRequestManager
            .RunHttpGetRequest(Arg.Any<string>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpRequestResult
            {
                ResultType = ERequestResult.Success,
                ResultContent = successJson
            }));

        var weatherApiService = new WeatherApiService(httpRequestManager, "your_app_id_here") { SpeedType = WeatherApiService.ESpeedType.MPH };

        // Act
        var task = (weatherApiService as IWeatherService).GetWeather(0.0, 0.0, 10.0f, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        Assert.AreEqual(ERequestResult.Success, result.ResultType);
        Assert.IsTrue(ApproxEqual(result.Weather.Temperature, 11));
        Assert.IsTrue(ApproxEqual(result.Weather.WindDirection, 130));
        Assert.IsTrue(ApproxEqual(result.Weather.WindSpeed, 5.6f));
    }

    private bool ApproxEqual(double d1, double d2)
    {
        return Math.Abs((d1 - d2)) <= 0.0001f;
    }

    [Test]
    public void GetWeather_ErrorResultType()
    {
        // Arrange
        var httpRequestManager = Substitute.For<IHttpRequestManager>();
        httpRequestManager
            .RunHttpGetRequest(Arg.Any<string>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpRequestResult
            {
                ResultType = ERequestResult.Error,
                Error = "Some error message"
            }));

        var weatherApiService = new WeatherApiService(httpRequestManager, "your_app_id_here");

        // Act
        var task = (weatherApiService as IWeatherService).GetWeather(0.0, 0.0, 10.0f, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        Assert.AreEqual(ERequestResult.Error, result.ResultType);
        Assert.AreEqual("Some error message", result.Error);
        // Add more assertions based on the expected behavior.
    }

    [Test]
    public void GetWeather_CancellationResultType()
    {
        // Arrange
        var httpRequestManager = Substitute.For<IHttpRequestManager>();
        httpRequestManager
            .RunHttpGetRequest(Arg.Any<string>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpRequestResult
            {
                ResultType = ERequestResult.Cancelled
            }));

        var weatherApiService = new WeatherApiService(httpRequestManager, "your_app_id_here");

        // Act
        var task = (weatherApiService as IWeatherService).GetWeather(0.0, 0.0, 10.0f, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        Assert.AreEqual(ERequestResult.Cancelled, result.ResultType);
    }

    [Test]
    public void GetWeather_TimedOutResultType()
    {
        // Arrange
        var httpRequestManager = Substitute.For<IHttpRequestManager>();
        httpRequestManager
            .RunHttpGetRequest(Arg.Any<string>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new HttpRequestResult
            {
                ResultType = ERequestResult.TimedOut
            }));

        var weatherApiService = new WeatherApiService(httpRequestManager, "your_app_id_here");

        // Act
        var task = (weatherApiService as IWeatherService).GetWeather(0.0, 0.0, 10.0f, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        Assert.AreEqual(ERequestResult.TimedOut, result.ResultType);
    }
}
