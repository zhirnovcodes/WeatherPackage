using System;

namespace Weather
{
    internal interface IComposition : IDisposable
    {
        IHttpRequestManager GetHttpRequestManager();

        ILocationProvider GetLocationProvider();

        ILog GetLog();
    }
}