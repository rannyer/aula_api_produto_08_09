using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ProdutosApi.Exceptions;

namespace ProdutosApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarAsync(context, ex);
        }
    }

    private static async Task TratarAsync(HttpContext httpContext, Exception exception)
    {
        var erro =  Mapear(exception);
        var problema = new ProblemDetails
        {
            Status = erro.status,
            Title = erro.titulo,
            Detail = erro.detalhe,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.io/{erro.status}"
        };
        
        httpContext.Response.Clear();
        httpContext.Response.StatusCode = erro.status;
        httpContext.Response.ContentType = "application/problem+json";
        
        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problema, _jsonOptions));
    }

    private static ErrorHttp Mapear(Exception exception) => exception switch
    {
        AppException app => new((int)app.StatusCode, app.Titulo, app.Message),
        _ => new(500, "Erro interno do servidor", "Ocorreu um erro inesperado")
    };

    private readonly record struct ErrorHttp(int status, string titulo, string detalhe);
}