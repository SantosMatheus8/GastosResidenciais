using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Infra.Queries;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Utils;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.User;

namespace ResiGa.Bkd.Infra.Repositories;

public class PessoaRepository(SqlConnection connection, ILogger<PessoaRepository> logger) : IPessoaRepository
{
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

    public async Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas)
    {
        var query = AddQueryPagination(listPessoas);
        var countQuery = AddFilters(listPessoas, PessoaQueries.Count);

        logger.LogInformation("Query a ser executada: {Sql}. with parameters: {@Parameters}", query, listPessoas);

        var parameters = new
        {
            Id = listPessoas.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade,
            Offset = (listPessoas.Page - 1) * listPessoas.ItemsPerPage,
            listPessoas.ItemsPerPage
        };

        var result = await connection.QueryAsync<Pessoa>(
            query,
            (pessoa, user) =>
            {
                pessoa.User = user;
                return pessoa;
            },
            parameters
        );

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

    private static string AddFilters(ListPessoas listPessoas, string query)
    {
        if (!string.IsNullOrEmpty(listPessoas.Nome))
            query += " AND LOWER(b.Nome) COLLATE Latin1_General_CI_AI LIKE '%' + @Nome + '%' ";
        if (listPessoas.Idade.HasValue)
            query += " AND b.Idade = @Idade ";
        return query;
    }

    public async Task<Pessoa> FindPessoaByIdAsync(decimal pessoaId, decimal userId)
    {
        logger.LogInformation("Query executada: {Sql}.", PessoaQueries.FindPessoaById);

        var result = await connection.QueryAsync<Pessoa>(
            PessoaQueries.FindPessoaById,
            (pessoa) =>
            {
                return pessoa;
            },
            new { PessoaId = pessoaId}
        );

        var pessoa = result.FirstOrDefault();

        logger.LogInformation("Resultado: {@Resultado}. ", pessoa);

        return pessoa!;
    }

    public async Task UpdatePessoaAsync(Pessoa pessoa)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", PessoaQueries.UpdatePessoa);
            var response = await connection.ExecuteAsync(PessoaQueries.UpdatePessoa,
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

    public async Task DeletePessoaAsync(decimal pessoaId)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", PessoaQueries.DeletePessoa);
            var response = await connection.ExecuteAsync(PessoaQueries.DeletePessoa,
                new
                {
                    PessoaId = pessoaId
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao deletar Pessoa : {Erro}", e);
            throw;
        }
    }
}
