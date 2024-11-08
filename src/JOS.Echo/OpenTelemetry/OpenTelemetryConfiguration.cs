using System;
using System.Collections.Generic;

namespace JOS.Echo.OpenTelemetry;

public class OpenTelemetryConfiguration
{
    public OpenTelemetryLogging Logging { get; init; }
    public OpenTelemetryMetrics Metrics { get; init; }
    public OpenTelemetryTracing Tracing { get; init; }
}

public class OpenTelemetryLogging
{
    public required bool Enabled { get; init; } = false;
    public required bool ConsoleEnabled { get; init; } = false;
    public required bool OLTPEnabled { get; init; } = false;
    public required OLTPExporterConfiguration OLTPExporter { get; init; }
}

public class OpenTelemetryMetrics
{
    public required bool Enabled { get; init; } = false;
    public required bool ConsoleEnabled { get; init; } = false;
    public required bool OLTPEnabled { get; init; } = false;
    public required OLTPExporterConfiguration OLTPExporter { get; init; }
}

public class OpenTelemetryTracing
{
    public required bool Enabled { get; init; } = false;
    public required bool ConsoleEnabled { get; init; } = false;
    public required bool OLTPEnabled { get; init; } = false;
    public required OLTPExporterConfiguration OLTPExporter { get; init; }
}

public class OLTPExporterConfiguration
{
    public required Uri Endpoint { get; init; }
}
