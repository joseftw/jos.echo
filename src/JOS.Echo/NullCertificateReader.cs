using System.Security.Cryptography.X509Certificates;

namespace JOS.Echo;

public class NullCertificateReader : ICertificateReader
{
    public X509Certificate2? Read()
    {
        return null;
    }
}
