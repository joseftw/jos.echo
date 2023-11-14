using JOS.Echo;
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
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind("server", serverConfiguration);
var certificateReader = new CachingMountedCertificateReader(new MountedCertificateReader(tlsConfiguration));
builder.Services.AddSingleton(certificateReader);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.All;
});
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

app.UseForwardedHeaders();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/", (HttpContext httpContext, CachingMountedCertificateReader cachingMountedCertificateReader)
    => EchoRequestHandler.Handle(httpContext, cachingMountedCertificateReader));

await app.RunAsync();
