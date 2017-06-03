# log4net.Extensions.AspNetCore

log4net extension for Asp.net Core. Easily add log4net logging into the dependency injection framework in Asp.net Core.

Supports NetStandard 1.5+.

## Quick Start

Install the `log4net.Extensions.AspNetCore` nuget package.

Add a log4net.xml file to your project, configure log4net settings, and set the file to copy to output. In the `Startup` class of an Asp.Net Core project, add the following:

```c#
...

using log4net.Extensions.AspNetCore;

...

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddLog4Net();
    loggerFactory.ConfigureLog4Net("log4net.xml");

    ...
}

```

## log4net Configuration

Using `loggerFactory.ConfigureLog4Net("log4net.xml")` is optional. Other ways to configure log4net will work with this extension. Adding the assembly attribute like below is another valid way to configure log4net:

```c#
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.xml", Watch = true)]
```

Basic configuration can be used with:

```c#
log4net.Config.BasicConfigurator.Configure(log4net.LogManager.GetRepository(Assembly.GetEntryAssembly()));
```

## Log Scopes

See [Aspnet Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging#log-scopes) for more details on Log Scopes.

This extension allows for log scopes. This feature is disabled by default. To enable scopes, pass `enableScopes: true` when calling `loggerFactory.AddLog4Net`. The below example shows a simple log scope:

```c#
ILogger l = loggerFactory.CreateLogger("boop");
l.LogInformation("Not in scope.");
using (var logScope = l.BeginScope("This will be appended."))
{
    l.LogInformation("This is info.");
}
```

This will use the following messages when logging.

```
Not in scope.
This is info.=> This will be appended.
```

## Notes

- The Microsoft.Extensions.Logging framework has trace level logging; however, log4net does not have this level. Any calls to `LogTrace` will be logged as Debug.

- You will see many logs from Microsoft.AspNetCore.* when using this extension due to the framework using registered logging provider. These can be removed by adding filters in the log4net configuration.