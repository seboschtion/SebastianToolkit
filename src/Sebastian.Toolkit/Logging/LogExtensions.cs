using MetroLog;

namespace Sebastian.Toolkit.Logging
{
    public static class LogExtensions
    {
        public static ILogger Logger(this object obj)
        {
            return new Logger(obj);
        }
    }
}
