using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    internal static class Helpers
    {
        public static string DoubleToString(double value)
        {
            return value.ToString().Replace(',', '.');
        }
    }

}
