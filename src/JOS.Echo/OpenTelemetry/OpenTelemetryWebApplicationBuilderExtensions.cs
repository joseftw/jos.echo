using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace JOS.Echo.OpenTelemetry;

public static class OpenTelemetryWebApplicationBuilderExtensions
{
    public static void AddOpenTelemetry(
        this WebApplicationBuilder builder, OpenTelemetryConfiguration openTelemetryConfiguration)
    {
        var openTelemetryBuilder = builder.Services.AddOpenTelemetry();
        openTelemetryBuilder.ConfigureResource(b => { b.AddService("JOS.Echo"); });
        if (openTelemetryConfiguration.Logging.Enabled)
        {
            openTelemetryBuilder.WithLogging(l =>
            {
                if (openTelemetryConfiguration.Logging.ConsoleEnabled)
                {
                    l.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Logging.OLTPEnabled)
                {
                    l.AddOtlpExporter(options =>
                    {
                        options.Endpoint = openTelemetryConfiguration.Logging.OLTPExporter.Endpoint;
                    });
                }
            });
        }

        if (openTelemetryConfiguration.Metrics.Enabled)
        {
            openTelemetryBuilder.WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
                m.AddRuntimeInstrumentation();
                m.AddHttpClientInstrumentation();
                if (openTelemetryConfiguration.Metrics.ConsoleEnabled)
                {
                    m.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Metrics.OLTPEnabled)
                {
                    m.AddOtlpExporter(options =>
                    {
                        options.Endpoint = openTelemetryConfiguration.Logging.OLTPExporter.Endpoint;
                    });
                }
            });
        }

        if (openTelemetryConfiguration.Tracing.Enabled)
        {
            openTelemetryBuilder.WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();
                if (openTelemetryConfiguration.Tracing.ConsoleEnabled)
                {
                    t.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Tracing.OLTPEnabled)
                {
                    t.AddOtlpExporter(options =>
                    {
                        options.Endpoint = openTelemetryConfiguration.Logging.OLTPExporter.Endpoint;
                    });
                }
            });
        }
    }
}
