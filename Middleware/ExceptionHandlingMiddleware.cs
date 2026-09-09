using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        AppException app => new((int)app.StatusCode, app.Titulo, 
            app.Message),
        DbUpdateException db when db.InnerException is PostgresException pg => 
            MapearPostgress(pg),
        DbUpdateException => new(400, "Falha ao gravar",
            "Nao foi possivel salvar as alteracoes no banco de dados"),
        NpgsqlException => new(500, "Erro de banco de dados", 
            "Ocorreu um erro ao acessar o banco de dados"),
        TaskCanceledException or OperationCanceledException=> new (400, 
            "Operação cancelada", "A operação foi cancelada"),
        ArgumentException => new (400, "Argumento inválido", 
            exception.Message),
        InvalidOperationException => new (422, "Operacao invalida",
            exception.Message),
        _ => new(500, "Erro interno do servidor", 
            "Ocorreu um erro inesperado")
    };
    
    private static ErrorHttp MapearPostgress(PostgresException pg) => pg.SqlState switch
    {
        PostgresErrorCodes.UniqueViolation => new (409, "Registro duplicado", 
            $"Ja existe um registre com esse valor (constraint: {pg.ConstraintName})"),
        PostgresErrorCodes.ForeignKeyViolation => new (409, "Referência invalida", 
            $"A operacao viola um relacionamento entre tabelas"),
        PostgresErrorCodes.NotNullViolation => new (400, "Campo obrigatório", 
            $"O campo {pg.ColumnName} é obrigatório"),
        PostgresErrorCodes.CheckViolation => new(422,"Restricao violada",
            $"A operacao viola a restricao (constraint: {pg.ConstraintName})"),
        _=> new(500, "Erro de banco de dados", "Ocorreu um ao acessar o banco de dados")
    };

    private readonly record struct ErrorHttp(int status, string titulo, string detalhe);
}