namespace Weather
{
    internal interface ILog
    {
        void Log(string log);
        void Enable();
        void Disable();
    }

}
