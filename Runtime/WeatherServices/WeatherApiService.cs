using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Weather
{
    public class WeatherApiService : IWeatherService
    {
        private IHttpRequestManager Http;
        private string AppId;

        public ETemperatureType TemperatureType { set; private get; } = ETemperatureType.Celsius;
        public ESpeedType SpeedType { set; private get; } = ESpeedType.KPH;

        public WeatherApiService(string appId) : this(Composition.GetHttpRequestManager(), appId)
        {
        }

        internal WeatherApiService(IHttpRequestManager http, string appId)
        {
            Http = http;
            AppId = appId;
        }

        async Task<WeatherRequestResultData> IWeatherService.GetWeather(double latitude, double longitude, float timeout, CancellationToken cancellation)
        {
            const string requestFormat = "http://api.weatherapi.com/v1/current.json?key={2}&q={0},{1}";

            var latString = Helpers.DoubleToString(latitude);
            var lonString = Helpers.DoubleToString(longitude);
            var appIdString = AppId.ToString();

            var request = string.Format(requestFormat, latString, lonString, appIdString);

            var result = await Http.RunHttpGetRequest(request, timeout, cancellation);

            var resultData = new WeatherRequestResultData();
            resultData.ResultType = result.ResultType;
            resultData.Error = result.Error;

            try
            {
                if (result.ResultType == ERequestResult.Success)
                {
                    var mapResult = JsonUtility.FromJson<WeatherApiResult>(result.ResultContent);
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

        private WeatherData ToWeatherData(WeatherApiResult apiResult)
        {
            var current = apiResult.current;

            var result = new WeatherData()
            {
                Temperature = TemperatureType == ETemperatureType.Celsius ? current.temp_c : current.temp_f,
                WindDirection = current.wind_degree,
                WindSpeed = SpeedType == ESpeedType.MPH ? current.wind_mph : current.wind_kph
            };

            return result;
        }

        public enum ETemperatureType
        {
            Celsius,
            Fahrenheit
        }

        public enum ESpeedType
        {
            /// <summary>
            /// Miles per hour
            /// </summary>
            MPH,
            KPH
        }

        [System.Serializable]
        internal class WeatherApiResult
        {
            public Current current;

            [System.Serializable]
            public class Current
            {
                public float temp_c;
                public float temp_f;
                public float wind_mph;
                public float wind_kph;
                public float wind_degree;
            }
        }
    }

}
