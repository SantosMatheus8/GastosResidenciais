namespace ResiGa.Bkd.Infra.Queries;

/// <summary>
/// Queries SQL para operacoes CRUD da entidade Transacao.
/// O campo Tipo armazena: 0=Despesa, 1=Receita.
/// </summary>
public static class TransacaoQueries
{
    /// <summary>
    /// Insere uma nova transacao e retorna o registro criado.
    /// CategoriaId e PessoaId sao chaves estrangeiras obrigatorias.
    /// </summary>
    public const string CreateTransacao = @"
        INSERT INTO Transacoes (Descricao, Valor, Tipo, CategoriaId, PessoaId)
        OUTPUT INSERTED.Id, INSERTED.Descricao, INSERTED.Valor, INSERTED.Tipo, INSERTED.CategoriaId, INSERTED.PessoaId
        VALUES (@Descricao, @Valor, @Tipo, @CategoriaId, @PessoaId)";

    /// <summary>
    /// Lista transacoes com suporte a filtros dinamicos.
    /// </summary>
    public const string ListTransacoes = @"
        SELECT
            t.Id,
            t.Descricao,
            t.Valor,
            t.Tipo,
            t.CategoriaId,
            t.PessoaId
        FROM Transacoes t WITH(NOLOCK)
        WHERE 1=1";

    /// <summary>
    /// Conta o total de transacoes para calculo de paginacao.
    /// </summary>
    public const string Count = @"
        SELECT COUNT(*)
        FROM Transacoes t WITH(NOLOCK)
        WHERE 1=1";

    /// <summary>
    /// Busca uma transacao pelo seu Id unico.
    /// </summary>
    public const string FindTransacaoById = @"
        SELECT
            Id,
            Descricao,
            Valor,
            Tipo,
            CategoriaId,
            PessoaId
        FROM Transacoes WITH(NOLOCK)
        WHERE Id = @TransacaoId";

    /// <summary>
    /// Atualiza os dados de uma transacao existente.
    /// </summary>
    public const string UpdateTransacao = @"
        UPDATE Transacoes
        SET Descricao = @Descricao,
            Valor = @Valor,
            Tipo = @Tipo,
            CategoriaId = @CategoriaId,
            PessoaId = @PessoaId
        WHERE Id = @TransacaoId";

    /// <summary>
    /// Deleta uma transacao pelo seu Id.
    /// </summary>
    public const string DeleteTransacao = @"
        DELETE FROM Transacoes
        WHERE Id = @TransacaoId";
}
