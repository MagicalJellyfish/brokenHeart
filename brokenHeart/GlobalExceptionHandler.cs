using System.Net;
using Microsoft.AspNetCore.Diagnostics;

namespace brokenHeart;

public class GlobalExceptionHandler : IExceptionHandler
{
    public GlobalExceptionHandler() { }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "text/plain";
        await httpContext.Response.WriteAsync(
            $"Internal Error: {exception.GetType().Name} - {exception.Message}",
            cancellationToken
        );

        return true;
    }
}
