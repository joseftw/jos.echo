using System;
using System.Linq;
using System.Net.Quic;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace JOS.Echo;

public static class EchoRequestHandler
{
    private static readonly string? FileVersion;
    private static readonly string? InformationalVersion;

    static EchoRequestHandler()
    {
        var assembly = typeof(Program).Assembly;
        FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        InformationalVersion =
            assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }

    public static IResult Handle(HttpContext httpContext, ICertificateReader certificateReader)
    {
        var certificate = certificateReader.Read();
        var data = new
        {
            Meta = new
            {
                QuickListener = QuicListener.IsSupported,
                Version = new
                {
                    Dotnet = new
                    {
                        Environment.Version
                    },
                    FileVersion,
                    InformationalVersion
                }
            },
            Certificate = certificate is null
                ? null
                : new
                {
                    certificate.NotBefore,
                    certificate.NotAfter,
                    certificate.SerialNumber,
                    Subject = certificate.SubjectName.Name,
                    certificate.Thumbprint
                },
            Request = new
            {
                Connection = new
                {
                    httpContext.Connection.Id,
                    httpContext.Connection.LocalPort,
                    httpContext.Connection.RemotePort,
                    LocalIp = httpContext.Connection.LocalIpAddress?.ToString(),
                    RemoteIp = httpContext.Connection.RemoteIpAddress?.ToString()
                },
                Headers = httpContext.Request.Headers.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value),
                httpContext.Request.Protocol
            }
        };

        return Results.Ok(data);
    }
}
