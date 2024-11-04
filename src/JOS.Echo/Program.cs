using JOS.Echo;
using JOS.Echo.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    new SerilogConfigurator(
            context.Configuration, context.HostingEnvironment, ThisAssembly.AssemblyInformationalVersion)
        .Configure(loggerConfiguration);
});

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
});
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
app.Logger.LogInformation(
    "Starting application in {Environment}", builder.Environment.EnvironmentName);

app.UseForwardedHeaders();

app.MapGet("/health", () => Results.Ok());
app.MapGet("/", EchoRequestHandler.Handle);

try
{
    await app.RunAsync();

}
finally
{
    await Log.CloseAndFlushAsync();
}

