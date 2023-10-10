using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Weather
{
    internal class LocationProvider : ILocationProvider
    {
        public async Task<LocationServiceResponse> GetLocation(float timeout, CancellationToken token)
        {
            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
            {
                WeatherLog.Log("Location not enabled on device or app does not have permission to access location");

                return new LocationServiceResponse
                {
                    ResultType = ERequestResult.Error,
                    Error = "Location not enabled on device or app does not have permission to access location"
                };
            }

            // Starts the location service.
            Input.location.Start();

            // Waits until the location service initializes
            var timeRemains = timeout;

            try
            {
                token.ThrowIfCancellationRequested();
                const float minTimeDelaySeconds = 0.5f;
                while (Input.location.status == LocationServiceStatus.Initializing && timeRemains > 0)
                {
                    var timeDelaySeconds = Mathf.Min(minTimeDelaySeconds, timeRemains);
                    await Task.Delay((int)(timeDelaySeconds * 1000));
                    timeRemains -= timeDelaySeconds;
                }
            }
            catch (TaskCanceledException t)
            {
                WeatherLog.Log($"{GetType().Name}.{nameof(GetLocation)}. Cancel");

                Input.location.Stop();

                return new LocationServiceResponse
                {
                    ResultType = ERequestResult.Cancelled
                };
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (timeRemains <= 0)
            {
                Input.location.Stop();

                WeatherLog.Log($"{GetType().Name}.{nameof(GetLocation)}. Timeout");

                return new LocationServiceResponse
                {
                    ResultType = ERequestResult.TimedOut,
                };
            }

            switch (Input.location.status)
            {
                case LocationServiceStatus.Failed:
                default:
                    Input.location.Stop();

                    WeatherLog.Log($"{GetType().Name}.{nameof(GetLocation)}. Failed");

                    return new LocationServiceResponse
                    {
                        ResultType = ERequestResult.Error,
                        Error = "Unable to detect location"
                    };
                case LocationServiceStatus.Running:
                    WeatherLog.Log($"{GetType().Name}.{nameof(GetLocation)}. Location found: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);

                    Input.location.Stop();

                    return new LocationServiceResponse
                    {
                        ResultType = ERequestResult.Success,
                        ResponseData = new LocationData
                        {
                            Latitude = Input.location.lastData.latitude,
                            Longitude = Input.location.lastData.longitude
                        }
                    };
            }
        }
    }

}
