namespace ResiGa.Bkd.Domain.Exceptions;

public class NotFoundException(string message) : ResigaBaseException(message, 404);
