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

#if NETSTANDARD2_0
		public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder) {
			return loggingBuilder.AddLog4Net(false);
		}
		public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, bool enableScopes) {
			loggingBuilder.AddProvider(new log4netLogProvider(enableScopes));
			return loggingBuilder;
		}
		public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, string configurationPath) {
			return loggingBuilder.AddLog4Net(false, configurationPath, Assembly.GetEntryAssembly());
		}
		public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, string configurationPath, Assembly repositoryAssembly) {
			return loggingBuilder.AddLog4Net(false, configurationPath, repositoryAssembly);
		}
		public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, bool enableScopes, string configurationPath, Assembly repositoryAssembly) {
			loggingBuilder.AddLog4Net(enableScopes);
			loggingBuilder.ConfigureLog4Net(configurationPath, repositoryAssembly);
			return loggingBuilder;
		}

		public static ILoggingBuilder ConfigureLog4Net(this ILoggingBuilder loggingBuilder, string configurationPath) {
			loggingBuilder.ConfigureLog4Net(configurationPath, Assembly.GetEntryAssembly());
			return loggingBuilder;
		}

		public static ILoggingBuilder ConfigureLog4Net(this ILoggingBuilder loggingBuilder, string configurationPath, Assembly repositoryAssembly) {
			var logRepository = LogManager.GetRepository(repositoryAssembly);
			XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(configurationPath));
			return loggingBuilder;
		}
#endif

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
