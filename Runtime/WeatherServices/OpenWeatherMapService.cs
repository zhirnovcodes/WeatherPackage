using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Weather
{
    public class OpenWeatherMapService : IWeatherService
    {
        private IHttpRequestManager Http;
        private string AppId;

        public EMeasurementUnits MeasurementUnits { set; private get; } = EMeasurementUnits.Standard;

        public OpenWeatherMapService(string appId) : this(Composition.GetHttpRequestManager(), appId)
        {
        }

        internal OpenWeatherMapService(IHttpRequestManager http, string appId)
        {
            Http = http;
            AppId = appId;
        }

        async Task<WeatherRequestResultData> IWeatherService.GetWeather(double latitude, double longitude, float timeout, CancellationToken cancellation)
        {
            const string requestFormat = "https://api.openweathermap.org/data/3.0/onecall?lat={0}&lon={1}&appid={2}&units={3}";

            var latString = Helpers.DoubleToString(latitude);
            var lonString = Helpers.DoubleToString(longitude);
            var appIdString = AppId.ToString();
            var unit = UnitToString(MeasurementUnits);

            var request = string.Format(requestFormat, latString, lonString, appIdString, unit);

            var result = await Http.RunHttpGetRequest(request, timeout, cancellation);

            var resultData = new WeatherRequestResultData();
            resultData.ResultType = result.ResultType;
            resultData.Error = result.Error;

            try
            {
                if (result.ResultType == ERequestResult.Success)
                {
                    var mapResult = JsonUtility.FromJson<OpenWeatherMapResult>(result.ResultContent);
                    resultData.Weather = ToWeatherData(mapResult);
                }
            }
            catch (System.Exception e)
            {
                resultData.ResultType = ERequestResult.Error;
                resultData.Error = "JSON cast exception: " + e.Message;
            }

            return resultData;
        }

        private string UnitToString(EMeasurementUnits units)
        {
            return units.ToString().ToLower();
        }

        private WeatherData ToWeatherData(OpenWeatherMapResult mapResult)
        {
            var result = new WeatherData()
            {
                Temperature = mapResult.current.temp,
                WindDirection = mapResult.current.wind_deg,
                WindSpeed = mapResult.current.wind_speed
            };

            return result;
        }

        public enum EMeasurementUnits
        {
            /// <summary>
            /// Default value. Kelvin temperature and meter/sec
            /// </summary>
            Standard,
            /// <summary>
            /// Cellsius temperature and meter/sec
            /// </summary>
            Metric,
            /// <summary>
            /// Fahrenheit temperature and miles/hour
            /// </summary>
            Imperial
        }

        [System.Serializable]
        internal class OpenWeatherMapResult
        {
            public Current current = new Current();

            [System.Serializable]
            public class Current
            {
                public float temp;
                public float humidity;
                public float wind_speed;
                public float wind_deg;
            }
        }
    }

}
