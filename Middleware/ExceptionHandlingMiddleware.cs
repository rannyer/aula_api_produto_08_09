using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ProdutosApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var problema = new ProblemDetails
            {
                Status = 500,
                Title = "Erro Interno",
                Detail = ex.Message,
            };

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/probelm+json";
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(problema));
        }
    }
}