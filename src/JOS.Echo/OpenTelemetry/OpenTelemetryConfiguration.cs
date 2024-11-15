using System;

namespace JOS.Echo.OpenTelemetry;

public class OpenTelemetryConfiguration
{
    public OtelLogsConfiguration Logs { get; init; } = new();
    public OtelMetricsConfiguration Metrics { get; init; } = new();
    public OtelTracesConfiguration Traces { get; init; } = new();

    public class OtelLogsConfiguration
    {
        public bool Enabled { get; init; } = false;
        public bool ConsoleEnabled { get; init; } = false;
        public bool OLTPEnabled { get; init; } = false;
        public Uri? Endpoint { get; init; }
    }

    public class OtelMetricsConfiguration
    {
        public bool Enabled { get; init; } = false;
        public bool ConsoleEnabled { get; init; } = false;
        public bool OLTPEnabled { get; init; } = false;
        public Uri? Endpoint { get; init; }
    }

    public class OtelTracesConfiguration
    {
        public bool Enabled { get; init; } = false;
        public bool ConsoleEnabled { get; init; } = false;
        public bool OLTPEnabled { get; init; } = false;
        public Uri? Endpoint { get; init; }
    }
}