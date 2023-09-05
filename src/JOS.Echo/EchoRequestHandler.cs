namespace JOS.Echo;

public static class EchoRequestHandler
{
    public static IResult Handle(HttpContext httpContext)
    {
        return Results.Ok(new
        {
            Request = new
            {
                httpContext.Request.Headers
            }
        });
    }
}
