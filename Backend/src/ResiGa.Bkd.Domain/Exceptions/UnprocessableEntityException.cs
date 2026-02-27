namespace ResiGa.Bkd.Domain.Exceptions;

public class UnprocessableEntityException(string message) : ResigaBaseException(message, 422);
