using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Weather
{
    internal class HttpRequestManager : IHttpRequestManager
    {
        private HttpClient Client;

        public HttpRequestManager(HttpClient client)
        {
            Client = client;
        }

        public async Task<HttpRequestResult> RunHttpGetRequest(string request, float secondsTimeout, CancellationToken cToken)
        {
            WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - secondsTimeout: {secondsTimeout}. Request:");
            WeatherLog.Log(request);

            var result = new HttpRequestResult();

            try
            {
                using (var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cToken))
                {
                    timeoutSource.CancelAfter(TimeSpan.FromSeconds(secondsTimeout));

                    var httpResponse = await Client.GetAsync(request, timeoutSource.Token);

                    const int maxLoggedRequestLength = 30;
                    request = request.Length <= maxLoggedRequestLength ? request : (request.Remove(maxLoggedRequestLength - 1) + "...");

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - Success! ({request}) Content: ");
                        result.ResultType = ERequestResult.Success;
                        result.ResultContent = await httpResponse.Content.ReadAsStringAsync();
                        WeatherLog.Log(result.ResultContent);
                    }
                    else
                    {
                        result.ResultType = ERequestResult.Error;
                        result.Error = $"HTTP Error: {httpResponse.StatusCode} - {httpResponse.ReasonPhrase}";
                        WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - Error! ({request}) {result.Error}");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (cToken.IsCancellationRequested)
                {
                    WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - Cancelled! ({request})");
                    result.ResultType = ERequestResult.Cancelled;
                }
                else
                {
                    WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - TimedOut! ({request})");
                    result.ResultType = ERequestResult.TimedOut;
                }
            }
            catch (Exception ex)
            {
                WeatherLog.Log($"{this.GetType().Name}.{nameof(RunHttpGetRequest)} - Exception! {ex.Message}");
                result.ResultType = ERequestResult.Error;
                result.Error = ex.Message;
            }

            return result;
        }
    }

}
