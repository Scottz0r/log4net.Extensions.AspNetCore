using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Reflection;
using log4net.Repository;

namespace log4net.Extensions.AspNetCore
{
    public class log4netLogger : ILogger
    {

        private readonly ILog _logger;
        public bool EnableScopes { get; set; }
        public string Name { get; set; }

        public log4netLogger(ILoggerRepository loggerRepository, string categoryName, bool enableScopes)
        {
            _logger = LogManager.GetLogger(loggerRepository.Name, categoryName);
            EnableScopes = enableScopes;
            Name = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            return log4netScope.Push(Name, state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return LoggerEnagled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!LoggerEnagled(logLevel))
            {
                return;
            }

            string message;
            if(formatter != null)
            {
                message = formatter(state, exception);
            }
            else
            {
                message = state?.ToString();
            }

            if(EnableScopes)
            {
                message = AppendScopeInformation(message);
            }

            GetLoggerAct(logLevel)(message, exception);
        }

        private bool LoggerEnagled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return _logger.IsDebugEnabled;
                case LogLevel.Information:
                    return _logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return _logger.IsWarnEnabled;
                case LogLevel.Error:
                    return _logger.IsErrorEnabled;
                case LogLevel.Critical:
                    return _logger.IsFatalEnabled;
                case LogLevel.None:
                default:
                    return false;
            }
        }

        private Action<string, Exception> GetLoggerAct(LogLevel logLevel)
        {
            switch(logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return _logger.Debug;
                case LogLevel.Information:
                    return _logger.Info;
                case LogLevel.Warning:
                    return _logger.Warn;
                case LogLevel.Error:
                    return _logger.Error;
                case LogLevel.Critical:
                    return _logger.Fatal;
                case LogLevel.None:
                default:
                    return (s, e) => { };
            }
        }

        private string AppendScopeInformation(string message)
        {
            var current = log4netScope.Current;

            if(current != null)
            {
                return message + $"=> {current}";
            }
            else
            {
                return message;
            }
        }
    }
}
