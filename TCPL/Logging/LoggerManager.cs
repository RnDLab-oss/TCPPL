
using NLog;
using ILogger = NLog.ILogger;

namespace API.Logging
{
    public class LoggerManager : ILoggerManager
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc/>
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }

        /// <inheritdoc/>
        public void LogError(string message)
        {
            logger.Error(message);
        }

        /// <inheritdoc/>
        public void LogInfo(string message)
        {
            logger.Info(message);
        }

        /// <inheritdoc/>
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
    }
}
