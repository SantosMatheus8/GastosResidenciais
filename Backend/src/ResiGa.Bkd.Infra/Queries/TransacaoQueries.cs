namespace ResiGa.Bkd.Infra.Queries;

public static class TransacaoQueries
{
    public const string CreateTransacao = @"
        INSERT INTO Transacoes (Descricao, Valor, Tipo, CategoriaId, PessoaId)
        OUTPUT INSERTED.Id, INSERTED.Descricao, INSERTED.Valor, INSERTED.Tipo, INSERTED.CategoriaId, INSERTED.PessoaId
        VALUES (@Descricao, @Valor, @Tipo, @CategoriaId, @PessoaId)";

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

    public const string Count = @"
        SELECT COUNT(*)
        FROM Transacoes t WITH(NOLOCK)
        WHERE 1=1";

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

    public const string UpdateTransacao = @"
        UPDATE Transacoes
        SET Descricao = @Descricao,
            Valor = @Valor,
            Tipo = @Tipo,
            CategoriaId = @CategoriaId,
            PessoaId = @PessoaId
        WHERE Id = @TransacaoId";

    public const string DeleteTransacao = @"
        DELETE FROM Transacoes
        WHERE Id = @TransacaoId";
}
