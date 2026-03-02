namespace ResiGa.Bkd.Domain.Exceptions;

/// <summary>
/// Excecao base do sistema ResiGa.
/// Todas as excecoes de negocio herdam desta classe.
/// Carrega o StatusCode HTTP que sera retornado ao cliente pelo ExceptionMiddleware.
/// Exemplos: NotFoundException (404), UnprocessableEntityException (422).
/// </summary>
public class ResigaBaseException : Exception
{
    public int StatusCode { get; set; }

    public ResigaBaseException(string? message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
