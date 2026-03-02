using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Infra.Queries;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Utils;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Infra.Repositories;

/// <summary>
/// Repositorio responsavel pelo acesso a dados da entidade Pessoa.
/// Utiliza Dapper para execucao de queries SQL parametrizadas.
/// </summary>
public class PessoaRepository(SqlConnection connection, ILogger<PessoaRepository> logger) : IPessoaRepository
{
    /// <summary>
    /// Cria uma nova pessoa no banco de dados.
    /// Utiliza transacao para garantir atomicidade da operacao.
    /// </summary>
    public async Task<Pessoa> CreatePessoaAsync(Pessoa pessoa)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query a ser executada: {Sql}.", PessoaQueries.CreatePessoa);
            var pessoaCreated = await connection.QueryAsync<Pessoa>(PessoaQueries.CreatePessoa, new
            {
                Nome = pessoa.Nome,
                Idade = pessoa.Idade
            }, transaction);
            await transaction.CommitAsync();

            return pessoaCreated.FirstOrDefault()!;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao Criar Pessoa : {Erro}", e);
            throw;
        }
    }

    /// <summary>
    /// Lista pessoas com suporte a filtros e paginacao.
    /// Constroi a query dinamicamente conforme os filtros informados.
    /// </summary>
    public async Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas)
    {
        var query = AddQueryPagination(listPessoas);
        var countQuery = AddFilters(listPessoas, PessoaQueries.Count);

        logger.LogInformation("Query a ser executada: {Sql}. with parameters: {@Parameters}", query, listPessoas);

        var parameters = new
        {
            Id = listPessoas.Id,
            Nome = listPessoas.Nome,
            Idade = listPessoas.Idade,
            Offset = (listPessoas.Page - 1) * listPessoas.ItemsPerPage,
            listPessoas.ItemsPerPage
        };

        var result = await connection.QueryAsync<Pessoa>(query, parameters);

        var totalLines = await connection.QuerySingleAsync<int>(countQuery, parameters);

        logger.LogInformation("Resultado: {@Resultado}. ", result);

        return new PaginatedResult<Pessoa>
        {
            Lines = result.ToList(),
            Page = listPessoas.Page,
            TotalPages = (int)Math.Ceiling(totalLines / (double)listPessoas.ItemsPerPage),
            TotalItens = totalLines,
            PageSize = listPessoas.ItemsPerPage
        };
    }

    /// <summary>
    /// Adiciona clausulas de ordenacao e paginacao (OFFSET/FETCH NEXT) a query.
    /// </summary>
    private static string AddQueryPagination(ListPessoas listPessoas)
    {
        var query = AddFilters(listPessoas, PessoaQueries.ListPessoas);
        query +=
            @$"
                ORDER BY
                {listPessoas.OrderBy.GetEnumDescription()}
                {listPessoas.OrderDirection.GetEnumDescription()}
                OFFSET @Offset
                ROWS FETCH NEXT @ItemsPerPage ROWS ONLY
                ";
        return query;
    }

    /// <summary>
    /// Adiciona filtros dinamicos a query base.
    /// Filtros com valor nulo ou vazio sao ignorados.
    /// </summary>
    private static string AddFilters(ListPessoas listPessoas, string query)
    {
        if (!string.IsNullOrEmpty(listPessoas.Nome))
            query += " AND LOWER(b.Nome) COLLATE Latin1_General_CI_AI LIKE '%' + @Nome + '%' ";
        if (listPessoas.Idade > 0)
            query += " AND b.Idade = @Idade ";
        return query;
    }

    /// <summary>
    /// Busca uma pessoa pelo seu Id. Retorna null se nao encontrada.
    /// </summary>
    public async Task<Pessoa?> FindPessoaByIdAsync(Guid pessoaId)
    {
        logger.LogInformation("Query executada: {Sql}.", PessoaQueries.FindPessoaById);

        var result = await connection.QueryAsync<Pessoa>(
            PessoaQueries.FindPessoaById,
            new { PessoaId = pessoaId }
        );

        var pessoa = result.FirstOrDefault();

        logger.LogInformation("Resultado: {@Resultado}. ", pessoa);

        return pessoa!;
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// </summary>
    public async Task UpdatePessoaAsync(Pessoa pessoa)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", PessoaQueries.UpdatePessoa);
            await connection.ExecuteAsync(PessoaQueries.UpdatePessoa,
                new
                {
                    PessoaId = pessoa.Id,
                    Nome = pessoa.Nome,
                    Idade = pessoa.Idade,
                    UpdatedAt = DateTime.Now
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao editar Pessoa : {Erro}", e);
            throw;
        }
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transacoes em cascata.
    /// REGRA DE NEGOCIO: ao deletar uma pessoa, todas as transacoes associadas
    /// devem ser removidas primeiro para manter a integridade referencial.
    /// Ambas as operacoes sao executadas dentro da mesma transacao.
    /// </summary>
    public async Task DeletePessoaAsync(Guid pessoaId)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            // Primeiro deleta todas as transacoes da pessoa (cascata)
            logger.LogInformation("Deletando transacoes em cascata para PessoaId: {PessoaId}", pessoaId);
            await connection.ExecuteAsync(PessoaQueries.DeleteTransacoesByPessoaId,
                new { PessoaId = pessoaId }, transaction);

            // Depois deleta a pessoa
            logger.LogInformation("Query executada: {Sql}.", PessoaQueries.DeletePessoa);
            await connection.ExecuteAsync(PessoaQueries.DeletePessoa,
                new { PessoaId = pessoaId }, transaction);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao deletar Pessoa : {Erro}", e);
            throw;
        }
    }

    /// <summary>
    /// Busca os totais financeiros agrupados por pessoa usando query SQL com LEFT JOIN.
    /// Retorna todas as pessoas, incluindo as que nao possuem transacoes (com valores zerados).
    /// </summary>
    public async Task<List<TotalPorPessoa>> GetTotaisPorPessoaAsync()
    {
        logger.LogInformation("Buscando totais por pessoa");

        var result = await connection.QueryAsync<TotalPorPessoa>(PessoaQueries.TotaisPorPessoa);

        return result.ToList();
    }
}
