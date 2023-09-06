using JOS.Echo;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var tlsConfiguration = new TlsConfiguration();
builder.Configuration.Bind("tls", tlsConfiguration);
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind("server", serverConfiguration);
var providedCertificateQuery = new MountedCertificateReader(tlsConfiguration);
builder.Services.AddSingleton(providedCertificateQuery);
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(serverConfiguration.Port, listenOptions =>
    {
        if (tlsConfiguration.HasCertificate)
        {

            listenOptions.Protocols = HttpProtocols.Http2 | HttpProtocols.Http3;
            listenOptions.UseHttps(httpsOptions =>
            {
                httpsOptions.ServerCertificateSelector = (_, _) => providedCertificateQuery.Read();
            });
        }
        else
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        }
    });
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/", (HttpContext httpContext, MountedCertificateReader providedCertificateQuery)
    => EchoRequestHandler.Handle(httpContext, providedCertificateQuery));

await app.RunAsync();
