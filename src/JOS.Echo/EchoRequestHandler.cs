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
                Subject = certificate?.SubjectName.Name
            },
            Request = new
            {
                httpContext.Request.Headers,
                Protocol = httpContext.Request.Protocol
            }
        });
    }
}
