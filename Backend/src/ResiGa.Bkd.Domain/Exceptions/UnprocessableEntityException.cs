namespace ResiGa.Bkd.Domain.Exceptions;

/// <summary>
/// Excecao lancada quando uma regra de negocio e violada ou um campo e invalido.
/// Retorna HTTP 422 (Unprocessable Entity) ao cliente.
/// Usada para: validacao de tamanho de campos, valor negativo, menor de idade,
/// incompatibilidade de categoria x tipo de transacao.
/// </summary>
public class UnprocessableEntityException(string message) : ResigaBaseException(message, 422);
