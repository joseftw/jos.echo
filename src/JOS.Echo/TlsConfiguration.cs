namespace JOS.Echo;

public class TlsConfiguration
{
    public bool HasCertificate => Certificate is not null;
    public CertificateConfiguration? Certificate { get; set; }
}

public class CertificateConfiguration
{
    public bool HasKeyFile => !string.IsNullOrWhiteSpace(KeyFile);
    public bool HasPassword => !string.IsNullOrWhiteSpace(Password);
    public required string CertificateFile { get; init; }
    public string? KeyFile { get; init; }
    public string? Password { get; init; }
}
