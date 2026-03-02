namespace ResiGa.Bkd.Domain.Exceptions;

/// <summary>
/// Excecao lancada quando um recurso nao e encontrado no banco de dados.
/// Retorna HTTP 404 (Not Found) ao cliente.
/// Usada em operacoes de busca por Id, update e delete.
/// </summary>
public class NotFoundException(string message) : ResigaBaseException(message, 404);
