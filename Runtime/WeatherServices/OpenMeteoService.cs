using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Weather
{
    public class OpenMeteoService : IWeatherService
    {
        private IHttpRequestManager Http;

        public ETemperatureUnit TemperatureUnit { set; private get; } = ETemperatureUnit.Celsius;
        public EWindSpeedUnit WindSpeedUnit { set; private get; } = EWindSpeedUnit.KMH;

        public OpenMeteoService() : 
            this(Composition.GetHttpRequestManager())
        {
        }

        internal OpenMeteoService(IHttpRequestManager http)
        {
            Http = http;
        }

        async Task<WeatherRequestResultData> IWeatherService.GetWeather(double latitude, double longitude, float timeout, CancellationToken cancellation)
        {
            const string requestFormat = "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&current_weather=true&forecast_days=1&temperature_unit={2}&windspeed_unit={3}";
            
            var latString = Helpers.DoubleToString(latitude);
            var lonString = Helpers.DoubleToString(longitude);
            var tempUnitString = TempUnitToString();
            var windSpeedToString = WindSpeedUnitToString();

            var request = String.Format(requestFormat, latString, lonString, tempUnitString, windSpeedToString);
            var result = await Http.RunHttpGetRequest(request, timeout, cancellation);

            return FromRequest(result);
        }

        private string TempUnitToString()
        {
            return TemperatureUnit.ToString().ToLower();
        }

        private string WindSpeedUnitToString()
        {
            return WindSpeedUnit.ToString().ToLower();
        }

        private WeatherRequestResultData FromRequest(HttpRequestResult result)
        {
            var error = result.Error;
            var resultType = result.ResultType;
            var content = result.ResultContent;

            WeatherData data = null;

            if (resultType == ERequestResult.Success)
            {
                try
                {
                    data = FromJson(content);
                }
                catch(Exception e)
                {
                    resultType = ERequestResult.Error;
                    error = "JSON cast exception: " + e.Message;
                }
            }

            return new WeatherRequestResultData
            {
                ResultType = resultType,
                Error = error,
                Weather = data
            };
        }

        private WeatherData FromJson(string json)
        {
            var response = JsonUtility.FromJson<OpenMeteoServiceResponse>(json);

            return new WeatherData
            {
                Temperature = response.current_weather.temperature,
                WindSpeed = response.current_weather.windspeed,
                WindDirection = response.current_weather.winddirection
            };
        }

        [System.Serializable]
        internal class OpenMeteoServiceResponse
        {
            public CurrentWeather current_weather = new CurrentWeather();

            [System.Serializable]
            public class CurrentWeather
            {
                public float temperature;
                public float windspeed;
                public float winddirection;
                public int weathercode;
            }
        }

        public enum ETemperatureUnit
        {
            Celsius,
            Fahrenheit
        }

        public enum EWindSpeedUnit
        {
            KMH,
            MS,
            MPH,
            KN
        }
    }


}
