using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Infra.Queries;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Utils;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Infra.Repositories;

public class TransacaoRepository(SqlConnection connection, ILogger<TransacaoRepository> logger) : ITransacaoRepository
{
    public async Task<Transacao> CreateTransacaoAsync(Transacao transacao)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query a ser executada: {Sql}.", TransacaoQueries.CreateTransacao);
            var transacaoCreated = await connection.QueryAsync<Transacao>(TransacaoQueries.CreateTransacao, new
            {
                Descricao = transacao.Descricao,
                Valor = transacao.Valor,
                Tipo = transacao.Tipo,
                CategoriaId = transacao.CategoriaId,
                PessoaId = transacao.PessoaId
            }, transaction);
            await transaction.CommitAsync();

            return transacaoCreated.FirstOrDefault()!;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao Criar Transacao : {Erro}", e);
            throw;
        }
    }

    public async Task<PaginatedResult<Transacao>> GetTransacoesAsync(ListTransacoes listTransacoes)
    {
        var query = AddQueryPagination(listTransacoes);
        var countQuery = AddFilters(listTransacoes, TransacaoQueries.Count);

        logger.LogInformation("Query a ser executada: {Sql}. with parameters: {@Parameters}", query, listTransacoes);

        var parameters = new
        {
            Id = listTransacoes.Id,
            Descricao = listTransacoes.Descricao,
            Tipo = listTransacoes.Tipo,
            CategoriaId = listTransacoes.CategoriaId,
            PessoaId = listTransacoes.PessoaId,
            Offset = (listTransacoes.Page - 1) * listTransacoes.ItemsPerPage,
            listTransacoes.ItemsPerPage
        };

        var result = await connection.QueryAsync<Transacao>(query, parameters);

        var totalLines = await connection.QuerySingleAsync<int>(countQuery, parameters);

        logger.LogInformation("Resultado: {@Resultado}. ", result);

        return new PaginatedResult<Transacao>
        {
            Lines = result.ToList(),
            Page = listTransacoes.Page,
            TotalPages = (int)Math.Ceiling(totalLines / (double)listTransacoes.ItemsPerPage),
            TotalItens = totalLines,
            PageSize = listTransacoes.ItemsPerPage
        };
    }

    private static string AddQueryPagination(ListTransacoes listTransacoes)
    {
        var query = AddFilters(listTransacoes, TransacaoQueries.ListTransacoes);
        query +=
            @$"
                ORDER BY
                {listTransacoes.OrderBy.GetEnumDescription()}
                {listTransacoes.OrderDirection.GetEnumDescription()}
                OFFSET @Offset
                ROWS FETCH NEXT @ItemsPerPage ROWS ONLY
                ";
        return query;
    }

    private static string AddFilters(ListTransacoes listTransacoes, string query)
    {
        if (!string.IsNullOrEmpty(listTransacoes.Descricao))
            query += " AND LOWER(t.Descricao) COLLATE Latin1_General_CI_AI LIKE '%' + @Descricao + '%' ";
        if (listTransacoes.Tipo > 0)
            query += " AND t.Tipo = @Tipo ";
        if (listTransacoes.CategoriaId.HasValue)
            query += " AND t.CategoriaId = @CategoriaId ";
        if (listTransacoes.PessoaId.HasValue)
            query += " AND t.PessoaId = @PessoaId ";
        return query;
    }

    public async Task<Transacao?> FindTransacaoByIdAsync(Guid transacaoId)
    {
        logger.LogInformation("Query executada: {Sql}.", TransacaoQueries.FindTransacaoById);

        var result = await connection.QueryAsync<Transacao>(
            TransacaoQueries.FindTransacaoById,
            new { TransacaoId = transacaoId }
        );

        var transacao = result.FirstOrDefault();

        logger.LogInformation("Resultado: {@Resultado}. ", transacao);

        return transacao!;
    }

    public async Task UpdateTransacaoAsync(Transacao transacao)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", TransacaoQueries.UpdateTransacao);
            await connection.ExecuteAsync(TransacaoQueries.UpdateTransacao,
                new
                {
                    TransacaoId = transacao.Id,
                    Descricao = transacao.Descricao,
                    Valor = transacao.Valor,
                    Tipo = transacao.Tipo,
                    CategoriaId = transacao.CategoriaId,
                    PessoaId = transacao.PessoaId
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao editar Transacao : {Erro}", e);
            throw;
        }
    }

    public async Task DeleteTransacaoAsync(Guid transacaoId)
    {
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            logger.LogInformation("Query executada: {Sql}.", TransacaoQueries.DeleteTransacao);
            await connection.ExecuteAsync(TransacaoQueries.DeleteTransacao,
                new
                {
                    TransacaoId = transacaoId
                }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("Erro ao deletar Transacao : {Erro}", e);
            throw;
        }
    }
}
