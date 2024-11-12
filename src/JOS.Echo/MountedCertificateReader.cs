using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace JOS.Echo;

public class MountedCertificateReader : ICertificateReader
{
    private static readonly ActivitySource source = new("JOS.MountedCertificateReader", "1.0.0");
    private readonly TlsConfiguration _tlsConfiguration;

    public MountedCertificateReader(TlsConfiguration tlsConfiguration)
    {
        _tlsConfiguration = tlsConfiguration ?? throw new ArgumentNullException(nameof(tlsConfiguration));
    }

    public X509Certificate2 Read()
    {
        using var activity = source.StartActivity("ReadCertificate", ActivityKind.Internal);
        {
            if (!_tlsConfiguration.HasCertificate)
            {
                throw new Exception("No certificate has been configured");
            }

            var certificate = _tlsConfiguration.Certificate!;
            if (!certificate.HasKeyFile)
            {
                return X509Certificate2.CreateFromPemFile(certificate.CertificateFile);
            }

            if (certificate.HasPassword)
            {
                return X509Certificate2.CreateFromEncryptedPemFile(
                    certificate.CertificateFile,
                    certificate.Password,
                    certificate.KeyFile);
            }

            return X509Certificate2.CreateFromPemFile(
                certificate.CertificateFile,
                certificate.KeyFile);
        }
    }
}
