using JOS.Configuration;
using JOS.Echo;
using JOS.Echo.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var tlsConfiguration = new TlsConfiguration();
builder.Configuration.Bind("tls", tlsConfiguration);
builder.Services.AddSingleton<ICertificateReader>(x =>
{
    if (tlsConfiguration.HasCertificate)
    {
        return new CachingMountedCertificateReader(new MountedCertificateReader(tlsConfiguration));
    }
    else
    {
        return new NullCertificateReader();
    }
});
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind("server", serverConfiguration);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
    options.ForwardLimit = null;
});
var openTelemetryConfiguration =
    builder.Configuration.GetRequiredOptions<OpenTelemetryConfiguration>("OTEL:EXPORTER:OTLP");
builder.AddOpenTelemetry(openTelemetryConfiguration);
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(serverConfiguration.Port, listenOptions =>
    {
        var certificateReader = listenOptions.ApplicationServices.GetRequiredService<ICertificateReader>();
        if (tlsConfiguration.HasCertificate)
        {
            listenOptions.Protocols = HttpProtocols.Http2 | HttpProtocols.Http3;
            listenOptions.UseHttps(httpsOptions =>
            {
                httpsOptions.ServerCertificateSelector = (_, _) => certificateReader.Read();
            });
        }
        else
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        }
    });
});

var app = builder.Build();
app.Logger.LogInformation("Starting application in {Environment}", builder.Environment.EnvironmentName);

app.UseForwardedHeaders();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/", EchoRequestHandler.Handle);

app.Logger.LogInformation("Let's do this Josef");

await app.RunAsync();
