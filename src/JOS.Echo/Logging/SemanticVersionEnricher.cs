using System;
using Serilog.Core;
using Serilog.Events;

namespace JOS.Echo.Logging;

public class SemanticVersionEnricher : ILogEventEnricher
{
    private readonly string _semanticVersion;

    public SemanticVersionEnricher(string semanticVersion)
    {
        _semanticVersion = semanticVersion ?? throw new ArgumentNullException(nameof(semanticVersion));
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        logEvent.AddPropertyIfAbsent(new LogEventProperty("semanticVersion", new ScalarValue(_semanticVersion)));
    }
}
