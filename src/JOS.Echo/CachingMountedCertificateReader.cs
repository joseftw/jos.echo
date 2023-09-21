using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace JOS.Echo;

public class CachingMountedCertificateReader
{
    private static readonly SemaphoreSlim Semaphore;
    private readonly MountedCertificateReader _certificateReader;
    private static DateTime? LastRefreshed;
    private static X509Certificate2? _certificate;

    static CachingMountedCertificateReader()
    {
        Semaphore = new SemaphoreSlim(1, 1);
    }

    public CachingMountedCertificateReader(MountedCertificateReader certificateReader)
    {
        _certificateReader = certificateReader ?? throw new ArgumentNullException(nameof(certificateReader));
    }

    public X509Certificate2 Read()
    {
        if (!NeedsRefresh(_certificate))
        {
            return _certificate!;
        }

        Semaphore.Wait();

        if (!NeedsRefresh(_certificate))
        {
            Semaphore.Release(1);
            return _certificate!;
        }

        _certificate = _certificateReader.Read();
        LastRefreshed = DateTime.UtcNow;
        Semaphore.Release(1);
        return _certificate;
    }

    private static bool NeedsRefresh(X509Certificate2? certificate)
    {
        return certificate is null || certificate.NotAfter <= DateTime.UtcNow || (DateTime.UtcNow - LastRefreshed!.Value).TotalMinutes >= 60;
    }
}
