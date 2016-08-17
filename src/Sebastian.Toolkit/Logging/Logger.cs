using System;
using MetroLog;

namespace Sebastian.Toolkit.Logging
{
    internal class Logger : ILogger
    {
        private readonly object _loggingObject;

        public Logger(object target)
        {
            _loggingObject = target;
        }

        private ILogger GetLogger()
        {
            string name = _loggingObject?.GetType().FullName ?? "unknown object";
            var result = LogManagerFactory.DefaultLogManager.GetLogger(name);
            return result;
        }

        public void Trace(string message, Exception ex = null)
        {
            GetLogger().Trace(message, ex);
        }

        public void Trace(string message, params object[] ps)
        {
            GetLogger().Trace(message, ps);
        }

        public void Debug(string message, Exception ex = null)
        {
            GetLogger().Debug(message, ex);
        }

        public void Debug(string message, params object[] ps)
        {
            GetLogger().Debug(message, ps);
        }

        public void Info(string message, Exception ex = null)
        {
            GetLogger().Info(message, ex);
        }

        public void Info(string message, params object[] ps)
        {
            GetLogger().Info(message, ps);
        }

        public void Warn(string message, Exception ex = null)
        {
            GetLogger().Warn(message, ex);
        }

        public void Warn(string message, params object[] ps)
        {
            GetLogger().Warn(message, ps);
        }

        public void Error(string message, Exception ex = null)
        {
            GetLogger().Error(message, ex);
        }

        public void Error(string message, params object[] ps)
        {
            GetLogger().Error(message, ps);
        }

        public void Fatal(string message, Exception ex = null)
        {
            GetLogger().Fatal(message, ex);
        }

        public void Fatal(string message, params object[] ps)
        {
            GetLogger().Fatal(message, ps);
        }

        public void Log(LogLevel logLevel, string message, Exception ex)
        {
            GetLogger().Log(logLevel, message, ex);
        }

        public void Log(LogLevel logLevel, string message, params object[] ps)
        {
            GetLogger().Log(logLevel, message, ps);
        }

        public bool IsEnabled(LogLevel level)
        {
            return GetLogger().IsEnabled(level);
        }

        public string Name { get; private set; }
        public bool IsTraceEnabled { get; private set; }
        public bool IsDebugEnabled { get; private set; }
        public bool IsInfoEnabled { get; private set; }
        public bool IsWarnEnabled { get; private set; }
        public bool IsErrorEnabled { get; private set; }
        public bool IsFatalEnabled { get; private set; }
    }
}
