using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] 
namespace Weather
{
    internal interface ILocationProvider
    {
        Task<LocationServiceResponse> GetLocation(float timeout, CancellationToken token);
    }
}