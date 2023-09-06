namespace JOS.Echo;

public static class EchoRequestHandler
{
    public static IResult Handle(HttpContext httpContext, MountedCertificateReader mountedCertificateReader)
    {
        var certificate = mountedCertificateReader.Read();
        return Results.Ok(new
        {
            Version = new
            {
                Assembly = ThisAssembly.AssemblyVersion,
                FileVersion = ThisAssembly.AssemblyFileVersion,
                Informational = ThisAssembly.AssemblyInformationalVersion
            },
            Certificate = new
            {
                NotBefore = certificate?.NotBefore,
                NotAfter = certificate?.NotAfter,
                SerialNumber = certificate?.SerialNumber,
                Subject = certificate?.SubjectName.Name,
                Thumbprint = certificate?.Thumbprint
            },
            Request = new
            {
                Headers = httpContext.Request.Headers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                Protocol = httpContext.Request.Protocol
            }
        });
    }
}
