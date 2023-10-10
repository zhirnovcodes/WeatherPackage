using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Weather
{
    public interface IWeatherService
    {
        internal Task<WeatherRequestResultData> GetWeather(double latitude, double longitude, float timeout, CancellationToken cancellation);
    }

}