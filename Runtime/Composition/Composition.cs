namespace Weather
{
    internal static class Composition
    {
        private static IComposition Instance = new ReleaseComposition();

        public static void SetComposition(IComposition composition)
        {
            Instance = composition;
        }

        public static IHttpRequestManager GetHttpRequestManager()
        {
            return Instance.GetHttpRequestManager();
        }

        public static ILocationProvider GetLocationProvider()
        {
            return Instance.GetLocationProvider();
        }

        public static ILog GetLog()
        {
            return Instance.GetLog();
        }

        public static void Dispose()
        {
            Instance.Dispose();
        }
    }
}