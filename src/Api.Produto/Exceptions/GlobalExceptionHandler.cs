using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Exceptions;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Exceptions;

[ExcludeFromCodeCoverage]
public class GlobalExceptionHandler() : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BaseException e)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = e.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        Log.Error("Path: {Path} - Status: {Status} - Error: {ProblemDetailsTitle}",
            problemDetails.Instance,
            problemDetails.Status,
            problemDetails.Title);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
