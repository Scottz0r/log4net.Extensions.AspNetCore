using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Reflection;
using log4net.Config;

namespace log4net.Extensions.AspNetCore
{
    public static class log4netExtensions
    {
        public static void AddLog4Net(this ILoggerFactory loggerFactory, bool enableScopes = false)
        {
            loggerFactory.AddProvider(new log4netLogProvider(enableScopes: enableScopes));
        }

        public static void ConfigureLog4Net(this ILoggerFactory loggerFactory, string configurationPath)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(configurationPath));
        }
    }
}
