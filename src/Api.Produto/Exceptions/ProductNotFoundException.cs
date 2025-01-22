using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Exceptions;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Exceptions;

[ExcludeFromCodeCoverage]
public class ProductNotFoundExceptionHandler() : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Verifica se a exceção é do tipo esperado
        if (exception is not ProductNotFoundException e)
        {
            return false; // Permite que outro handler processe o erro
        }

        // Cria a resposta detalhada do problema
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = e.Message,
            Status = StatusCodes.Status404NotFound,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        };

        // Define o código de status da resposta
        httpContext.Response.StatusCode = problemDetails.Status.Value;

        // Loga o erro
        Log.Error("Path: {Path} - Status: {Status} - Error: {ProblemDetailsTitle}", 
            problemDetails.Instance, 
            problemDetails.Status,
            problemDetails.Title);

        // Retorna a resposta JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true; // Indica que o erro foi tratado
    }
}

public class ProductInvalidExceptionHandler(ILogger<ProductInvalidExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Verifica se a exceção é do tipo esperado
        if (exception is not ProductInvalidException e)
        {
            return false; // Permite que outro handler processe o erro
        }

        // Cria a resposta detalhada do problema
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = e.Message,
            Status = (int)e.StatusCode
        };

        // Define o código de status da resposta
        httpContext.Response.StatusCode = (int)e.StatusCode;
        httpContext.Response.ContentType = "application/json";

        // Loga o erro
        Log.Error("Error: {ProblemDetailsTitle}", problemDetails.Title);

        // Retorna a resposta JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true; // Indica que o erro foi tratado
    }
}