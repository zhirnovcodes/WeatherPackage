using System.Collections.Generic;

namespace Weather
{
    public class Weather
    {
        public List<WeatherRequestResultData> ResultList = new List<WeatherRequestResultData>();
    }

    public class WeatherRequestResultData
    {
        public ERequestResult ResultType;
        public string Error;
        public WeatherData Weather;
    }

    public class WeatherData
    {
        /// <summary>
        /// Temerature in chosen unit measurment type. Chose type with Services' properties
        /// </summary>
        public float Temperature;
        /// <summary>
        /// Wind speed in chosen unit measurment type. Chose type with Services' properties (if has)
        /// </summary>
        public float WindSpeed;
        /// <summary>
        /// Wind direction in degrees
        /// </summary>
        public float WindDirection;
    }
}
