using Microsoft.Extensions.Logging;
using System;

namespace Dios.Services
{
    public class Log<T> : ILog<T>
    {
        private readonly ILogger<T> logger;

        public Log(ILogger<T> logger)
        {
            this.logger = logger;
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            logger.LogError(ex, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            logger.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            logger.LogWarning(message, args);
        }
    }
}
