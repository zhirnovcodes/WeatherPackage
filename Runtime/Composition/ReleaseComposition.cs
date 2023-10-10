using System;
using System.Net.Http;

namespace Weather
{
    internal class ReleaseComposition : IComposition, IDisposable
    {
        private IHttpRequestManager HttpRequest;
        private ILocationProvider Location;
        private ILog Log;

        public void Dispose()
        {
            HttpRequest = null;
            Location = null;
            Log = null;
        }

        public IHttpRequestManager GetHttpRequestManager()
        {
            if (HttpRequest == null)
            {
                var client = new HttpClient();
                HttpRequest = new HttpRequestManager(client);
            }

            return HttpRequest;
        }

        public ILocationProvider GetLocationProvider()
        {
            if (Location == null)
            {
                Location = new LocationProvider();
            }
            return Location;
        }

        public ILog GetLog()
        {
            if (Log == null)
            {
                Log = new UnityLog();
            }

            return Log;
        }
    }
}