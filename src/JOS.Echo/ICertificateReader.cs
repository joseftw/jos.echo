using System.Security.Cryptography.X509Certificates;

namespace JOS.Echo;

public interface ICertificateReader
{
    X509Certificate2? Read();
}
