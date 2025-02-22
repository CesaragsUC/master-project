using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Exceptions;

[ExcludeFromCodeCoverage]
public class InvalidExceptionHandler(ILogger<InvalidExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Verifica se a exceção é do tipo esperado
        if (exception is not InvalidOperationException e)
        {
            logger.LogWarning("⚠️ Exceção não tratada pelo InvalidExceptionHandler: {ExceptionType}", exception.GetType().Name);
            return false; // Permite que outro handler processe o erro
        }

        // Cria a resposta detalhada do problema
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path,
            Title = "An unexpected error occurred! Please try again later.",
            Status = StatusCodes.Status500InternalServerError,

        };

        // Define o código de status da resposta
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        // Loga o erro
        Log.Error("Error: {InvalidExceptionHandler}", problemDetails.Title);

        // Retorna a resposta JSON
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken).ConfigureAwait(false);

        return true; // Indica que o erro foi tratado
    }
}