using System.Reflection;

namespace JOS.Echo;

public static class EchoRequestHandler
{
    private static readonly string FileVersion;
    private static readonly string InformationalVersion;

    static EchoRequestHandler()
    {
        var assembly = typeof(Program).Assembly;
        FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "n/a";
        InformationalVersion =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "n/a";
    }
    public static IResult Handle(HttpContext httpContext, CachingMountedCertificateReader certificateReader)
    {
        var certificate = certificateReader.Read();
        return Results.Ok(new
        {
            Version = new
            {
                FileVersion,
                InformationalVersion
            },
            Certificate = new
            {
                certificate.NotBefore,
                certificate.NotAfter,
                certificate.SerialNumber,
                Subject = certificate.SubjectName.Name,
                certificate.Thumbprint
            },
            Request = new
            {
                Connection = new
                {
                    httpContext.Connection.Id,
                    httpContext.Connection.LocalPort,
                    httpContext.Connection.RemotePort,
                    LocalIp = httpContext.Connection.LocalIpAddress?.ToString(),
                    RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString()
                },
                Headers = httpContext.Request.Headers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                httpContext.Request.Protocol
            }
        });
    }
}
