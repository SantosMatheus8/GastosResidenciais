namespace ResiGa.Bkd.Infra.Queries;

/// <summary>
/// Queries SQL para operacoes CRUD da entidade Pessoa.
/// Todas as queries utilizam parametros para prevenir SQL Injection.
/// </summary>
public static class PessoaQueries
{
    /// <summary>
    /// Insere uma nova pessoa e retorna o registro criado com o Id gerado automaticamente.
    /// </summary>
    public const string CreatePessoa = @"
        INSERT INTO Pessoas (Nome, Idade)
        OUTPUT INSERTED.Id, INSERTED.Nome, INSERTED.Idade
        VALUES (@Nome, @Idade)";

    /// <summary>
    /// Lista pessoas com suporte a filtros dinamicos (WHERE 1=1 permite concatenar AND).
    /// </summary>
    public const string ListPessoas = @"
        SELECT
            b.Id,
            b.Nome,
            b.Idade
        FROM Pessoas b WITH(NOLOCK)
        WHERE 1=1";

    /// <summary>
    /// Conta o total de pessoas para calculo de paginacao.
    /// </summary>
    public const string Count = @"
        SELECT COUNT(*)
        FROM Pessoas b WITH(NOLOCK)
        WHERE 1=1";

    /// <summary>
    /// Busca uma pessoa pelo seu Id unico.
    /// </summary>
    public const string FindPessoaById = @"
        SELECT
            Id,
            Nome,
            Idade
        FROM Pessoas WITH(NOLOCK)
        WHERE Id = @PessoaId";

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// </summary>
    public const string UpdatePessoa = @"
        UPDATE Pessoas
        SET Nome = @Nome,
            Idade = @Idade
        WHERE Id = @PessoaId";

    /// <summary>
    /// Deleta todas as transacoes associadas a uma pessoa.
    /// Deve ser executado ANTES de DeletePessoa para garantir a delecao em cascata
    /// conforme regra de negocio: ao deletar pessoa, todas as suas transacoes sao removidas.
    /// </summary>
    public const string DeleteTransacoesByPessoaId = @"
        DELETE FROM Transacoes
        WHERE PessoaId = @PessoaId";

    /// <summary>
    /// Deleta uma pessoa pelo seu Id.
    /// IMPORTANTE: deve ser executado APOS DeleteTransacoesByPessoaId para manter integridade.
    /// </summary>
    public const string DeletePessoa = @"
        DELETE FROM Pessoas
        WHERE Id = @PessoaId";

    /// <summary>
    /// Consulta de totais financeiros agrupados por pessoa.
    /// Calcula total de receitas (Tipo=1), total de despesas (Tipo=0) e saldo para cada pessoa.
    /// Inclui pessoas sem transacoes (LEFT JOIN) com valores zerados.
    /// </summary>
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
