using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace JOS.Echo.Logging;

public class SerilogConfigurator
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly string? _semanticVersion;

    public SerilogConfigurator(
        IConfiguration configuration, IHostEnvironment hostEnvironment, string? semanticVersion = null)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        _semanticVersion = semanticVersion ?? throw new ArgumentNullException(nameof(semanticVersion));
    }

    public void Configure(LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration.MinimumLevel.Is(DefaultLevel);

        ConfigureLogLevelPerNamespace(loggerConfiguration);

        loggerConfiguration.Enrich.FromLogContext();
        loggerConfiguration.Enrich.WithExceptionDetails();

        if(ShouldLogJson)
        {
            loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
        }
        else
        {
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}");
        }

        if(!string.IsNullOrWhiteSpace(_semanticVersion))
        {
            loggerConfiguration.Enrich.With(new SemanticVersionEnricher(_semanticVersion));
        }
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
        return _configuration.GetSection("Logging:LogLevel").Get<Dictionary<string, string>>()!;
    }

    private LogEventLevel DefaultLevel =>
        _configuration.GetValue<LogEventLevel?>("Logging:LogLevel:Default") ?? LogEventLevel.Debug;

    private bool ShouldLogJson =>
        _configuration.GetValue<bool?>("Logging:Output:Console:Json") ?? !_hostEnvironment.IsDevelopment();
}
