using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Infra.Queries;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Utils;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Infra.Repositories;

public class CategoriaRepository(SqlConnection connection, ILogger<CategoriaRepository> logger) : ICategoriaRepository
{
    public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query a ser executada: {Sql}.", CategoriaQueries.CreateCategoria);
            var categoriaCreated = await connection.QueryAsync<Categoria>(CategoriaQueries.CreateCategoria, new
            {
                Descricao = categoria.Descricao,
                Finalidade = categoria.Finalidade
            }, transaction);
            await transaction.CommitAsync();

            return categoriaCreated.FirstOrDefault()!;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao Criar Categoria : {Erro}", e);
            throw;
        }
    }

    public async Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias)
    {
        var query = AddQueryPagination(listCategorias);
        var countQuery = AddFilters(listCategorias, CategoriaQueries.Count);

        logger.LogInformation("Query a ser executada: {Sql}. with parameters: {@Parameters}", query, listCategorias);

        var parameters = new
        {
            Id = listCategorias.Id,
            Descricao = listCategorias.Descricao,
            Finalidade = listCategorias.Finalidade,
            Offset = (listCategorias.Page - 1) * listCategorias.ItemsPerPage,
            listCategorias.ItemsPerPage
        };

        var result = await connection.QueryAsync<Categoria>(query, parameters);

        var totalLines = await connection.QuerySingleAsync<int>(countQuery, parameters);

        logger.LogInformation("Resultado: {@Resultado}. ", result);

        return new PaginatedResult<Categoria>
        {
            Lines = result.ToList(),
            Page = listCategorias.Page,
            TotalPages = (int)Math.Ceiling(totalLines / (double)listCategorias.ItemsPerPage),
            TotalItens = totalLines,
            PageSize = listCategorias.ItemsPerPage
        };
    }

    private static string AddQueryPagination(ListCategorias listCategorias)
    {
        var query = AddFilters(listCategorias, CategoriaQueries.ListCategorias);
        query +=
            @$"
                ORDER BY
                {listCategorias.OrderBy.GetEnumDescription()}
                {listCategorias.OrderDirection.GetEnumDescription()}
                OFFSET @Offset
                ROWS FETCH NEXT @ItemsPerPage ROWS ONLY
                ";
        return query;
    }

    private static string AddFilters(ListCategorias listCategorias, string query)
    {
        if (!string.IsNullOrEmpty(listCategorias.Descricao))
            query += " AND LOWER(c.Descricao) COLLATE Latin1_General_CI_AI LIKE '%' + @Descricao + '%' ";
        if (listCategorias.Finalidade > 0)
            query += " AND c.Finalidade = @Finalidade ";
        return query;
    }

    public async Task<Categoria?> FindCategoriaByIdAsync(Guid categoriaId)
    {
        logger.LogInformation("Query executada: {Sql}.", CategoriaQueries.FindCategoriaById);

        var result = await connection.QueryAsync<Categoria>(
            CategoriaQueries.FindCategoriaById,
            new { CategoriaId = categoriaId }
        );

        var categoria = result.FirstOrDefault();

        logger.LogInformation("Resultado: {@Resultado}. ", categoria);

        return categoria!;
    }

    public async Task UpdateCategoriaAsync(Categoria categoria)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", CategoriaQueries.UpdateCategoria);
            await connection.ExecuteAsync(CategoriaQueries.UpdateCategoria,
                new
                {
                    CategoriaId = categoria.Id,
                    Descricao = categoria.Descricao,
                    Finalidade = categoria.Finalidade
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao editar Categoria : {Erro}", e);
            throw;
        }
    }

    public async Task DeleteCategoriaAsync(Guid categoriaId)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", CategoriaQueries.DeleteCategoria);
            await connection.ExecuteAsync(CategoriaQueries.DeleteCategoria,
                new
                {
                    CategoriaId = categoriaId
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao deletar Categoria : {Erro}", e);
            throw;
        }
    }
}
