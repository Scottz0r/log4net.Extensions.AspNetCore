using System;
using Xunit;
using Microsoft.Extensions.Logging;
using log4net.Extensions.AspNetCore;
using log4net.Appender;
using System.Reflection;
using log4net.Repository.Hierarchy;
using log4net.Config;
using System.IO;

namespace log4net.Extensions.AspNetCore.Tests
{
    public class TestLogger
    {
        private MemoryAppender MemAppend
        {
            get
            {
                var hierarchy = LogManager.GetRepository(Assembly.GetEntryAssembly()) as Hierarchy;
                return hierarchy.Root.GetAppender("MemoryAppender") as MemoryAppender;
            }
        }

        private ILogger GetTestLogger(bool enableScopes = false)
        {
            var factory = new LoggerFactory();
            factory.AddLog4Net(enableScopes: enableScopes);
            factory.ConfigureLog4Net("log4net.xml");
            return factory.CreateLogger("Test");
        }

        [Fact]
        public void It_should_log_trace_as_debug()
        {
            var logger = GetTestLogger();

            logger.LogTrace("This is a trace.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a trace.", events[0].MessageObject);
            Assert.Equal(Core.Level.Debug.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_log_debug()
        {
            var logger = GetTestLogger();

            logger.LogDebug("This is a debug.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a debug.", events[0].MessageObject);
            Assert.Equal(Core.Level.Debug.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_log_information()
        {
            ILogger logger = GetTestLogger();

            logger.LogInformation("This is a test message.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a test message.", events[0].MessageObject);
            Assert.Equal(Core.Level.Info.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_level_warning()
        {
            ILogger logger = GetTestLogger();

            logger.LogWarning("This is a warn message.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a warn message.", events[0].MessageObject);
            Assert.Equal(Core.Level.Warn.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_log_error()
        {
            ILogger logger = GetTestLogger();

            logger.LogError("This is a err message.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a err message.", events[0].MessageObject);
            Assert.Equal(Core.Level.Error.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_log_critical_as_fatal()
        {
            ILogger logger = GetTestLogger();

            logger.LogCritical("This is a fatal message.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a fatal message.", events[0].MessageObject);
            Assert.Equal(Core.Level.Fatal.Name, events[0].Level.Name);
        }

        [Fact]
        public void It_should_log_exception()
        {
            ILogger logger = GetTestLogger();
            var testException = new Exception("Test");

            logger.LogCritical(1, testException, "This is a fatal message.");

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("This is a fatal message.", events[0].MessageObject);
            Assert.Equal(Core.Level.Fatal.Name, events[0].Level.Name);
            Assert.Same(testException, events[0].ExceptionObject);
        }

        [Fact]
        public void It_should_not_log_none()
        {
            ILogger logger = GetTestLogger();

            //Wtf is log level none?
            logger.Log(LogLevel.None, 1, "Asdf", null, null);

            var events = MemAppend.PopAllEvents();

            Assert.Equal(0, events.Length);
        }

        [Fact]
        public void It_should_handle_scope()
        {
            ILogger logger = GetTestLogger(true);

            using (var scope = logger.BeginScope("Test Scope 1"))
            {
                logger.LogInformation("Test message.");
            }

            var events = MemAppend.PopAllEvents();
            Assert.Equal(1, events.Length);
            Assert.Equal("Test message. => Test Scope 1", events[0].MessageObject);
        }

        [Fact]
        public void It_should_handle_nested_scope()
        {
            ILogger logger = GetTestLogger(true);

            using (var scope1 = logger.BeginScope("Scope 1"))
            {
                using (var scope2 = logger.BeginScope("Scope 2"))
                {
                    logger.LogInformation("S2");
                }

                logger.LogInformation("S1");
            }

            var events = MemAppend.PopAllEvents();
            Assert.Equal(2, events.Length);
            Assert.Equal("S2 => Scope 2", events[0].MessageObject);
            Assert.Equal("S1 => Scope 1", events[1].MessageObject);
        }

        [Fact]
        public void It_should_not_use_scopes_disabled()
        {
            ILogger logger = GetTestLogger(false);

            using (var scope = logger.BeginScope("Asdf"))
            {
                logger.LogInformation("Test");
            }

            var events = MemAppend.PopAllEvents();
            Assert.Equal("Test", events[0].MessageObject);
        }

        [Fact]
        public void It_should_handle_null_message()
        {
            var logger = GetTestLogger();

            logger.LogInformation(null);

            //don't care about result, just want to be sure it doesn't raise exception.
        }
    }
}
