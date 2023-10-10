namespace Weather
{
    internal static class WeatherLog
    {
        public static void Log(string text)
        {
            Composition.GetLog().Log(text);
        }
    }

}
