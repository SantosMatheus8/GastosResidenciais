using System.Text.Json;
using ResiGa.Bkd.Domain.Exceptions;

namespace ResiGa.Bkd.Api.Middlewares;

/// <summary>
/// Middleware centralizado de tratamento de excecoes.
/// Intercepta todas as excecoes lancadas durante o pipeline HTTP e retorna respostas JSON padronizadas.
///
/// - ResigaBaseException (NotFoundException, UnprocessableEntityException): retorna o StatusCode especifico com a mensagem de erro.
/// - Excecoes genericas: retorna 500 com mensagem generica, sem expor detalhes internos.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ResigaBaseException ex)
        {
            // Excecoes de negocio: NotFoundException (404), UnprocessableEntityException (422)
            logger.LogWarning(ex, "Erro de negocio: {Message}", ex.Message);
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
        }
        catch (Exception ex)
        {
            // Excecoes inesperadas: retorna 500 sem expor detalhes internos
            logger.LogError(ex, "Erro inesperado");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Erro interno do servidor" }));
        }
    }
}
