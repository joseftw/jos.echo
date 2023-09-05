using JOS.Echo;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
var httpsConfiguration = new HttpsConfiguration();
builder.Configuration.Bind("https", httpsConfiguration);
var serverConfiguration = new ServerConfiguration();
builder.Configuration.Bind("server", serverConfiguration);
builder.WebHost.ConfigureKestrel((_, options) =>
{
    options.ListenAnyIP(serverConfiguration.Port, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        if (httpsConfiguration.HasCertificate)
        {
            listenOptions.UseHttps(httpsConfiguration.Certificate);
        }
        else
        {
            listenOptions.UseHttps();
        }
    });
});

var app = builder.Build();

app.MapGet("/", (HttpContext httpContext) => EchoRequestHandler.Handle(httpContext));

await app.RunAsync();
