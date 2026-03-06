namespace ResiGa.Bkd.Domain.Exceptions;

public class ResigaBaseException : Exception
{
    public int StatusCode { get; set; }

    public ResigaBaseException(string? message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
