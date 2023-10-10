using UnityEngine;

namespace Weather
{
    internal class UnityLog : ILog
    {
        private bool IsEnabled = true;

        public void Disable()
        {
            IsEnabled = false;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Log(string log)
        {
            if (IsEnabled)
            {
                Debug.Log(log);
            }
        }
    }
}

