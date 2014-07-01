using System;

namespace Orchard.Logging
{
    /// <summary>
    /// Provides logging services.
    /// </summary>
    /// <remarks>
    /// The sole purpose of this service is to provide a way to be able to resolve an ILogger or inject it through constructors instead
    /// of relying on property injection.
    /// </remarks>
    public interface ILoggerService : ILogger
    {
    }


    public class LoggerService : ILoggerService
    {
        public ILogger Logger { get; set; }


        public bool IsEnabled(LogLevel level)
        {
            return Logger.IsEnabled(level);
        }

        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            Logger.Log(level, exception, format, args);
        }
    }
}