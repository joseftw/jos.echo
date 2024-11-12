using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace JOS.Echo;

public class NullCertificateReader : ICertificateReader
{
    public X509Certificate2? Read()
    {
        using var activity = OTELActivitySource.JOSEcho.Source.StartActivity("Read");
        activity?.SetTag("josef", "Test");
        return null;
    }
}
