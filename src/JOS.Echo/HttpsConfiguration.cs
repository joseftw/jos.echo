namespace JOS.Echo;

public class HttpsConfiguration
{
    public bool HasCertificate => !string.IsNullOrWhiteSpace(Certificate);
    public string Certificate { get; set; } = null!;
}
