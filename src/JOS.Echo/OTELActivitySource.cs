using System.Diagnostics;
using JOS.Enumeration;

namespace JOS.Echo;

public partial class OTELActivitySource : IEnumeration<string, OTELActivitySource>
{
    public static readonly OTELActivitySource JOSEcho = new("JOS.Echo", "JOS.Echo", new ActivitySource("JOS.Echo"));

    private OTELActivitySource(string value, string description, ActivitySource source) : this(value, description)
    {
        Source = source;
    }

    public ActivitySource Source { get; }

}
