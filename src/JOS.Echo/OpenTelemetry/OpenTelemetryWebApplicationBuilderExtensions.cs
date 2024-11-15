using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
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
        openTelemetryBuilder.ConfigureResource(b =>
        {
            b.AddService("JOS.Echo");
        });
        if (openTelemetryConfiguration.Logs.Enabled)
        {
            openTelemetryBuilder.WithLogging(l =>
            {
                if (openTelemetryConfiguration.Logs.ConsoleEnabled)
                {
                    l.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Logs.OLTPEnabled)
                {
                    l.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        options.Endpoint = openTelemetryConfiguration.Logs.Endpoint ??
                                           throw new Exception("Logs Endpoint can't be null if enabled");
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
                m.AddMeter("Microsoft.AspNetCore.Hosting");
                m.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                var meters = OTELActivitySource.GetAll();
                foreach (var meter in meters)
                {
                    m.AddMeter(meter);
                }
                if (openTelemetryConfiguration.Metrics.ConsoleEnabled)
                {
                    m.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Metrics.OLTPEnabled)
                {
                    m.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        options.Endpoint = openTelemetryConfiguration.Metrics?.Endpoint ??
                                           throw new Exception("Metrics Endpoint can't be null if enabled");
                    });
                }
            });
        }

        if (openTelemetryConfiguration.Traces.Enabled)
        {
            openTelemetryBuilder.WithTracing(t =>
            {
                t.AddAspNetCoreInstrumentation();
                t.AddHttpClientInstrumentation();
                var meters = OTELActivitySource.GetAll();
                foreach (var meter in meters)
                {
                    t.AddSource(meter);
                }
                if (openTelemetryConfiguration.Traces.ConsoleEnabled)
                {
                    t.AddConsoleExporter();
                }

                if (openTelemetryConfiguration.Traces.OLTPEnabled)
                {
                    t.AddOtlpExporter(options =>
                    {
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        options.Endpoint = openTelemetryConfiguration.Traces?.Endpoint ??
                                           throw new Exception("Traces Endpoint can't be null if enabled");;
                    });
                }
            });
        }
    }
}
