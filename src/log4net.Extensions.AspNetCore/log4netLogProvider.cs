using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using log4net.Repository;

namespace log4net.Extensions.AspNetCore
{
    public class log4netLogProvider : ILoggerProvider
    {
        private readonly ILoggerRepository _loggerRepository;
        public bool EnableScopes { get; set; }

        public log4netLogProvider(bool enableScopes = false)
        {
            _loggerRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            EnableScopes = enableScopes;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new log4netLogger(_loggerRepository, categoryName, EnableScopes);
        }

        public void Dispose()
        {
            //Do nothing.
        }
    }
}
