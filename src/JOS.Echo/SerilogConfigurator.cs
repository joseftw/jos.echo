using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using ILogger = Serilog.ILogger;

namespace JOS.Echo;

public class SerilogConfigurator
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly string _version;

    public SerilogConfigurator(
        IConfiguration configuration, IHostEnvironment hostEnvironment, string version)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _version = version ?? throw new ArgumentNullException(nameof(version));
    }

    public ILogger Configure()
    {
        var loggerConfiguration = new LoggerConfiguration();

        loggerConfiguration.MinimumLevel.Is(DefaultLevel);

        ConfigureLogLevelPerNamespace(loggerConfiguration);

        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.Enrich.WithProperty("__version", _version);

        if(ShouldLogJson)
        {
            loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
        }
        else
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}");
        }

        return loggerConfiguration.CreateLogger();
    }

    private void ConfigureLogLevelPerNamespace(LoggerConfiguration loggerConfiguration)
    {
        var logLevels = GetLogLevels();

        foreach(var (path, level) in logLevels)
        {
            if(Enum.TryParse(typeof(LogEventLevel), level, true, out var logEventLevel))
            {
                loggerConfiguration.MinimumLevel.Override(path, (LogEventLevel)logEventLevel!);
            }
        }
    }

    private Dictionary<string, string> GetLogLevels()
    {
        return _configuration.GetSection("Logging:LogLevel").Get<Dictionary<string, string>>()
               ?? new Dictionary<string, string>();
    }

    private LogEventLevel DefaultLevel =>
        _configuration.GetValue<LogEventLevel?>("Logging:LogLevel:Default") ?? LogEventLevel.Debug;

    private bool ShouldLogJson =>
        _configuration.GetValue<bool?>("Logging:Output:Console:Json") ?? !_hostEnvironment.IsDevelopment();
}
