namespace ResiGa.Bkd.Infra.Queries;

public static class PessoaQueries
{
    public const string CreatePessoa = @"
        INSERT INTO Pessoas (Nome, Idade)
        OUTPUT INSERTED.Id, INSERTED.Nome, INSERTED.Idade
        VALUES (@Nome, @Idade)";

    public const string ListPessoas = @"
        SELECT
            b.Id,
            b.Nome,
            b.Idade
        FROM Pessoas b WITH(NOLOCK)
        WHERE 1=1";

    public const string Count = @"
        SELECT COUNT(*)
        FROM Pessoas b WITH(NOLOCK)
        WHERE 1=1";

    public const string FindPessoaById = @"
        SELECT
            Id,
            Nome,
            Idade
        FROM Pessoas WITH(NOLOCK)
        WHERE Id = @PessoaId";

    public const string UpdatePessoa = @"
        UPDATE Pessoas
        SET Nome = @Nome,
            Idade = @Idade
        WHERE Id = @PessoaId";
    public const string DeleteTransacoesByPessoaId = @"
        DELETE FROM Transacoes
        WHERE PessoaId = @PessoaId";

    public const string DeletePessoa = @"
        DELETE FROM Pessoas
        WHERE Id = @PessoaId";

    public const string TotaisPorPessoa = @"
        SELECT
            p.Id AS PessoaId,
            p.Nome,
            ISNULL(SUM(CASE WHEN t.Tipo = 1 THEN t.Valor ELSE 0 END), 0) AS TotalReceitas,
            ISNULL(SUM(CASE WHEN t.Tipo = 0 THEN t.Valor ELSE 0 END), 0) AS TotalDespesas,
            ISNULL(SUM(CASE WHEN t.Tipo = 1 THEN t.Valor ELSE 0 END), 0)
            - ISNULL(SUM(CASE WHEN t.Tipo = 0 THEN t.Valor ELSE 0 END), 0) AS Saldo
        FROM Pessoas p WITH(NOLOCK)
        LEFT JOIN Transacoes t WITH(NOLOCK) ON t.PessoaId = p.Id
        GROUP BY p.Id, p.Nome
        ORDER BY p.Nome";
}
