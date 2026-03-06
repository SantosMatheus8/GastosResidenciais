namespace ResiGa.Bkd.Infra.Queries;

public static class CategoriaQueries
{
    public const string CreateCategoria = @"
        INSERT INTO Categorias (Descricao, Finalidade)
        OUTPUT INSERTED.Id, INSERTED.Descricao, INSERTED.Finalidade
        VALUES (@Descricao, @Finalidade)";

    public const string ListCategorias = @"
        SELECT
            c.Id,
            c.Descricao,
            c.Finalidade
        FROM Categorias c WITH(NOLOCK)
        WHERE 1=1";

    public const string Count = @"
        SELECT COUNT(*)
        FROM Categorias c WITH(NOLOCK)
        WHERE 1=1";

    public const string FindCategoriaById = @"
        SELECT
            Id,
            Descricao,
            Finalidade
        FROM Categorias WITH(NOLOCK)
        WHERE Id = @CategoriaId";

    public const string UpdateCategoria = @"
        UPDATE Categorias
        SET Descricao = @Descricao,
            Finalidade = @Finalidade
        WHERE Id = @CategoriaId";

    public const string DeleteTransacoesByCategoriaId = @"
        DELETE FROM Transacoes
        WHERE CategoriaId = @CategoriaId";
        
    public const string DeleteCategoria = @"
        DELETE FROM Categorias
        WHERE Id = @CategoriaId";

    public const string TotaisPorCategoria = @"
        SELECT
            c.Id AS CategoriaId,
            c.Descricao,
            c.Finalidade,
            ISNULL(SUM(CASE WHEN t.Tipo = 1 THEN t.Valor ELSE 0 END), 0) AS TotalReceitas,
            ISNULL(SUM(CASE WHEN t.Tipo = 0 THEN t.Valor ELSE 0 END), 0) AS TotalDespesas,
            ISNULL(SUM(CASE WHEN t.Tipo = 1 THEN t.Valor ELSE 0 END), 0)
            - ISNULL(SUM(CASE WHEN t.Tipo = 0 THEN t.Valor ELSE 0 END), 0) AS Saldo
        FROM Categorias c WITH(NOLOCK)
        LEFT JOIN Transacoes t WITH(NOLOCK) ON t.CategoriaId = c.Id
        GROUP BY c.Id, c.Descricao, c.Finalidade
        ORDER BY c.Descricao";
}
