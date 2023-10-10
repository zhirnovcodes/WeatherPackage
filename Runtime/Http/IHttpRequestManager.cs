using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    internal interface IHttpRequestManager
    {
        Task<HttpRequestResult> RunHttpGetRequest(string request, float secondsTimeout, CancellationToken cToken);
    }
}

