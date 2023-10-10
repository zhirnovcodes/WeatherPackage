using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    public class WeatherProvider : IDisposable
    {
        private List<IWeatherService> Services = new List<IWeatherService>();
        private ILocationProvider LocationProvider;

        public WeatherProvider() : 
            this(Composition.GetLocationProvider())
        {

        }

        internal WeatherProvider(ILocationProvider location)
        {
            LocationProvider = location;
        }

        public static void EnableLogs()
        {
            Composition.GetLog().Enable();
        }

        public static void DisableLogs()
        {
            Composition.GetLog().Disable();
        }

        public void AddService(IWeatherService service)
        {
            foreach (var existing in Services)
            {
                if (existing is INamedWeatherService existingNamedService)
                {
                    if (service is INamedWeatherService addingNamedService)
                    {
                        if (existingNamedService.GetName() == addingNamedService.GetName())
                        {
                            throw new Exception($"Service with name {existingNamedService.GetName()} already exists. Skipping");
                        }
                        continue;
                    }
                    continue;
                }

                if (existing.GetType().Equals(service.GetType()))
                {
                    throw new Exception($"Service of type {existing.GetType()} already exists. Skipping");
                }
            }

            Services.Add(service);
        }

        public async Task<Weather> GetWeather(double latitude, double longitude, float timeout, CancellationToken token)
        {
            var taskList = new List<Task<WeatherRequestResultData>>(Services.Count);
            var resultData = new Weather();

            foreach (var service in Services)
            {
                taskList.Add(service.GetWeather(latitude, longitude, timeout, token));
            }

            await Task.WhenAll(taskList);

            for (int i = 0; i < taskList.Count; i++)
            {
                var task = taskList[i];
                var result = task.Result;
                resultData.ResultList.Add(result);
            }

            return resultData;
        }

        public async Task<Weather> GetWeather(float timeout, CancellationToken token)
        {
            var time = UnityEngine.Time.time;
            var locationResult = await LocationProvider.GetLocation(timeout, token);
            var deltaTime = UnityEngine.Time.time - time;

            switch (locationResult.ResultType)
            {
                case ERequestResult.Success:
                    var lat = locationResult.ResponseData.Latitude;
                    var lon = locationResult.ResponseData.Longitude;
                    return await GetWeather(lat, lon, timeout - deltaTime, token);
                case ERequestResult.Error:
                case ERequestResult.Cancelled:
                case ERequestResult.TimedOut:
                    var result = new Weather();

                    for (int i = 0; i < Services.Count; i++)
                    {
                        var resultData = new WeatherRequestResultData
                        {
                            ResultType = locationResult.ResultType,
                            Error = locationResult.Error,

                        };
                        result.ResultList.Add(resultData);
                    }

                    return result;
                default:
                    throw new System.NotImplementedException(locationResult.ResultType.ToString());
            }
        }

        public void Dispose()
        {
            Composition.Dispose();
        }
    }

}
