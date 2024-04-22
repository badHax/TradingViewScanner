using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace TVScanner.Shared.Logging
{
    public class ApplicationLogger : IAbstractLogger
    {
        private readonly ILoggerFactory _factory;
        private readonly ConcurrentDictionary<Type, ILogger> _loggers = new ConcurrentDictionary<Type, ILogger>();

        public ApplicationLogger(ILoggerFactory factory)
        {
            _factory = factory;
        }
        public void LogError<T>(T classType, string message)
        {
            ILogger logger = GetLogger<T>();
            logger.LogError(message);
        }

        public void LogInformation<T>(T classType, string message)
        {
            ILogger logger = GetLogger<T>();
            logger.LogInformation(message);
        }

        private ILogger GetLogger<T>()
        {
            return _loggers.GetOrAdd(typeof(T), t => _factory.CreateLogger<T>());
        }
    }
}