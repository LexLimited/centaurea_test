namespace centaurea_test.Middlewares;

public class StaticFilesCompressionMiddleware
{
    private readonly RequestDelegate _next;

    public StaticFilesCompressionMiddleware(RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(next);
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/app/"))
        {
            await _next.Invoke(context);
            return;
        }

        if (context.Request.Headers.AcceptEncoding.Contains("br"))
        {
            context.Request.Path.Add(".br");
        }
        else if (context.Request.Headers.AcceptEncoding.Contains("gzip"))
        {
            context.Request.Path.Add(".gz");
        }

        await _next.Invoke(context);
    }
}