using System.Reflection;

namespace JOS.Echo;

public static class EchoRequestHandler
{
    private static readonly string Assembly;
    private static readonly string FileVersion;
    private static readonly string InformationalVersion;

    static EchoRequestHandler()
    {
        var assembly = typeof(Program).Assembly;

        Assembly = assembly.GetCustomAttribute<AssemblyVersionAttribute>()?.Version ?? "n/a";
        FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "n/a";
        InformationalVersion =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "n/a";
    }
    public static IResult Handle(HttpContext httpContext, MountedCertificateReader mountedCertificateReader)
    {
        var certificate = mountedCertificateReader.Read();
        return Results.Ok(new
        {
            Version = new
            {
                Assembly,
                FileVersion,
                InformationalVersion
            },
            Certificate = new
            {
                certificate?.NotBefore,
                certificate?.NotAfter,
                certificate?.SerialNumber,
                Subject = certificate?.SubjectName.Name,
                certificate?.Thumbprint
            },
            Request = new
            {
                Headers = httpContext.Request.Headers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                httpContext.Request.Protocol
            }
        });
    }
}
