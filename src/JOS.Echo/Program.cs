using System.Reflection;
using JOS.Echo;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var version =
    typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

builder.Logging.ClearProviders();
var logger = new SerilogConfigurator(builder.Configuration, builder.Environment, version).Configure();
builder.Logging.AddSerilog(logger);

var tlsConfiguration = new TlsConfiguration();
builder.Configuration.Bind("tls", tlsConfiguration);
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind("server", serverConfiguration);
var certificateReader = new CachingMountedCertificateReader(new MountedCertificateReader(tlsConfiguration));
builder.Services.AddSingleton(certificateReader);
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(serverConfiguration.Port, listenOptions =>
    {
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
app.Logger.LogInformation(
    "Starting application in {Environment}", builder.Environment.EnvironmentName);

app.MapGet("/health", () => Results.Ok());
app.MapGet("/", (HttpContext httpContext, CachingMountedCertificateReader cachingMountedCertificateReader)
    => EchoRequestHandler.Handle(httpContext, cachingMountedCertificateReader));

await app.RunAsync();
