using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Weather;

[TestFixture]
public class WeatherProviderTest
{
    [Test]
    public void TestGetWeatherWithServices()
    {
        // Arrange
        using var provider = new WeatherProvider();
        var service1 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service2 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service3 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service4 = Substitute.For<IWeatherService, INamedWeatherService>();

        var successData = new WeatherData
        {
            Temperature = 10,
            WindDirection = 20,
            WindSpeed = 30
        };

        service1.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Success, Weather = successData }));
        service2.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Cancelled }));
        service3.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Error, Error = "ERROR STR" }));
        service4.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.TimedOut }));

        (service1 as INamedWeatherService).GetName().Returns("N1");
        (service2 as INamedWeatherService).GetName().Returns("N2");
        (service3 as INamedWeatherService).GetName().Returns("N3");
        (service4 as INamedWeatherService).GetName().Returns("N4");

        provider.AddService(service1);
        provider.AddService(service2);
        provider.AddService(service3);
        provider.AddService(service4);

        // Act
        var task = provider.GetWeather(1, 2, 3, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        var s1 = service1.Received(1).GetWeather(1, 2, 3, Arg.Any<CancellationToken>());
        var s2 = service2.Received(1).GetWeather(1, 2, 3, Arg.Any<CancellationToken>());
        var s3 = service3.Received(1).GetWeather(1, 2, 3, Arg.Any<CancellationToken>());
        var s4 = service4.Received(1).GetWeather(1, 2, 3, Arg.Any<CancellationToken>());

        Assert.AreEqual(4, result.ResultList.Count);
        Assert.AreEqual(ERequestResult.Success, result.ResultList[0].ResultType);
        Assert.AreEqual(result.ResultList[0].Weather.Temperature, 10);
        Assert.AreEqual(result.ResultList[0].Weather.WindDirection, 20);
        Assert.AreEqual(result.ResultList[0].Weather.WindSpeed, 30);
        Assert.AreEqual(ERequestResult.Cancelled, result.ResultList[1].ResultType);
        Assert.AreEqual(ERequestResult.Error, result.ResultList[2].ResultType);
        Assert.IsTrue(result.ResultList[2].Error.Contains("ERROR STR"));
        Assert.AreEqual(ERequestResult.TimedOut, result.ResultList[3].ResultType);
    }

    [Test]
    public void TestGetWeatherWithLocationProvider()
    {
        // Arrange
        var locationProvider = Substitute.For<ILocationProvider>();
        locationProvider.GetLocation(Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new LocationServiceResponse { ResultType = ERequestResult.Success, ResponseData = new LocationData 
            {
                Latitude = 10,
                Longitude = 20
            } }));

        using var provider = new WeatherProvider(locationProvider);

        var service1 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service2 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service3 = Substitute.For<IWeatherService, INamedWeatherService>();
        var service4 = Substitute.For<IWeatherService, INamedWeatherService>();

        var successData = new WeatherData
        {
            Temperature = 10,
            WindDirection = 10,
            WindSpeed = 10
        };

        service1.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Success, Weather = successData }));
        service2.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Cancelled }));
        service3.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.Error, Error = "ERROR STR" }));
        service4.GetWeather(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new WeatherRequestResultData { ResultType = ERequestResult.TimedOut }));

        (service1 as INamedWeatherService).GetName().Returns("N1");
        (service2 as INamedWeatherService).GetName().Returns("N2");
        (service3 as INamedWeatherService).GetName().Returns("N3");
        (service4 as INamedWeatherService).GetName().Returns("N4");

        provider.AddService(service1);
        provider.AddService(service2);
        provider.AddService(service3);
        provider.AddService(service4);

        // Act
        var task = provider.GetWeather(5, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously(); 
        }
        var result = task.Result;

        // Assert
        var s1 = service1.Received(1).GetWeather(10, 20, Arg.Is<float>(t => t > 0 && t <= 5), Arg.Any<CancellationToken>());

        Assert.AreEqual(4, result.ResultList.Count);
        Assert.AreEqual(ERequestResult.Success, result.ResultList[0].ResultType);
        Assert.AreEqual(ERequestResult.Cancelled, result.ResultList[1].ResultType);
        Assert.AreEqual(ERequestResult.Error, result.ResultList[2].ResultType);
        Assert.AreEqual(ERequestResult.TimedOut, result.ResultList[3].ResultType);
    }

    [Test]
    public void TestGetWeatherWithLocationCancel()
    {
        // Arrange
        var locationProvider = Substitute.For<ILocationProvider>();
        locationProvider.GetLocation(Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new LocationServiceResponse
            {
                ResultType = ERequestResult.Cancelled
            }));

        using var provider = new WeatherProvider(locationProvider);

        var service1 = Substitute.For<IWeatherService>();

        provider.AddService(service1);

        // Act
        var task = provider.GetWeather(5, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        var s1 = service1.DidNotReceiveWithAnyArgs().GetWeather(0,0,0, Arg.Any<CancellationToken>());

        Assert.AreEqual(1, result.ResultList.Count);
        Assert.AreEqual(ERequestResult.Cancelled, result.ResultList[0].ResultType);
    }

    [Test]
    public void TestGetWeatherWithLocationFail()
    {
        // Arrange
        var locationProvider = Substitute.For<ILocationProvider>();
        locationProvider.GetLocation(Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new LocationServiceResponse
            {
                ResultType = ERequestResult.Error,
                Error = "Err!!!"
            }));

        using var provider = new WeatherProvider(locationProvider);

        var service1 = Substitute.For<IWeatherService>();

        provider.AddService(service1);

        // Act
        var task = provider.GetWeather(5, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        var s1 = service1.DidNotReceiveWithAnyArgs().GetWeather(0, 0, 0, Arg.Any<CancellationToken>());

        Assert.AreEqual(1, result.ResultList.Count);
        Assert.AreEqual(ERequestResult.Error, result.ResultList[0].ResultType);
        Assert.IsTrue(result.ResultList[0].Error.Contains("Err!!!"));
    }

    [Test]
    public void TestGetWeatherWithLocationTimeout()
    {
        // Arrange
        var locationProvider = Substitute.For<ILocationProvider>();
        locationProvider.GetLocation(Arg.Any<float>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new LocationServiceResponse
            {
                ResultType = ERequestResult.TimedOut
            }));

        using var provider = new WeatherProvider(locationProvider);

        var service1 = Substitute.For<IWeatherService>();

        provider.AddService(service1);

        // Act
        var task = provider.GetWeather(5, CancellationToken.None);
        if (!task.IsCompleted)
        {
            task.RunSynchronously();
        }
        var result = task.Result;

        // Assert
        var s1 = service1.DidNotReceiveWithAnyArgs().GetWeather(0, 0, 0, Arg.Any<CancellationToken>());

        Assert.AreEqual(1, result.ResultList.Count);
        Assert.AreEqual(ERequestResult.TimedOut, result.ResultList[0].ResultType);
    }
}