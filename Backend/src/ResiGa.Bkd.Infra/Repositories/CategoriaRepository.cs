using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Infra.Queries;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Utils;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Infra.Repositories;

/// <summary>
/// Repositorio responsavel pelo acesso a dados da entidade Categoria.
/// Utiliza Dapper para execucao de queries SQL parametrizadas.
/// </summary>
public class CategoriaRepository(SqlConnection connection, ILogger<CategoriaRepository> logger) : ICategoriaRepository
{
    /// <summary>
    /// Cria uma nova categoria no banco de dados.
    /// O campo Finalidade define se a categoria aceita Despesa (0), Receita (1) ou Ambas (2).
    /// </summary>
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

    /// <summary>
    /// Lista categorias com suporte a filtros e paginacao.
    /// </summary>
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

    /// <summary>
    /// Adiciona clausulas de ordenacao e paginacao a query.
    /// </summary>
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

    /// <summary>
    /// Adiciona filtros dinamicos a query base.
    /// </summary>
    private static string AddFilters(ListCategorias listCategorias, string query)
    {
        if (!string.IsNullOrEmpty(listCategorias.Descricao))
            query += " AND LOWER(c.Descricao) COLLATE Latin1_General_CI_AI LIKE '%' + @Descricao + '%' ";
        if (listCategorias.Finalidade > 0)
            query += " AND c.Finalidade = @Finalidade ";
        return query;
    }

    /// <summary>
    /// Busca uma categoria pelo seu Id. Retorna null se nao encontrada.
    /// </summary>
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

    /// <summary>
    /// Atualiza os dados de uma categoria existente.
    /// </summary>
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

    /// <summary>
    /// Deleta uma categoria e todas as suas transacoes em cascata.
    /// Primeiro remove as transacoes associadas, depois a categoria.
    /// Ambas as operacoes sao executadas dentro da mesma transacao SQL.
    /// </summary>
    public async Task DeleteCategoriaAsync(Guid categoriaId)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Deletando transacoes em cascata para CategoriaId: {CategoriaId}", categoriaId);
            await connection.ExecuteAsync(CategoriaQueries.DeleteTransacoesByCategoriaId,
                new { CategoriaId = categoriaId }, transaction);

            logger.LogInformation("Query executada: {Sql}.", CategoriaQueries.DeleteCategoria);
            await connection.ExecuteAsync(CategoriaQueries.DeleteCategoria,
                new { CategoriaId = categoriaId }, transaction);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao deletar Categoria : {Erro}", e);
            throw;
        }
    }

    /// <summary>
    /// Busca os totais financeiros agrupados por categoria usando query SQL com LEFT JOIN.
    /// Retorna todas as categorias, incluindo as que nao possuem transacoes (com valores zerados).
    /// </summary>
    public async Task<List<TotalPorCategoria>> GetTotaisPorCategoriaAsync()
    {
        logger.LogInformation("Buscando totais por categoria");

        var result = await connection.QueryAsync<TotalPorCategoria>(CategoriaQueries.TotaisPorCategoria);

        return result.ToList();
    }
}
