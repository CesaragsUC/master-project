using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Product.Domain.Exceptions;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Product.Api.Exceptions;

[ExcludeFromCodeCoverage]
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