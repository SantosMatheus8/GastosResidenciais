using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Service;

public class TransacaoService(ITransacaoRepository repository, ILogger<TransacaoService> logger) : ITransacaoService
{
    public async Task<Transacao> CreateTransacaoAsync(Transacao transacao)
    {
        logger.LogInformation("Criando Transacao");

        return await repository.CreateTransacaoAsync(transacao);
    }

    public async Task<PaginatedResult<Transacao>> GetTransacoesAsync(ListTransacoes listTransacoes)
    {
        logger.LogInformation("Listando Transacao");
        return await repository.GetTransacoesAsync(listTransacoes);
    }

    public async Task<Transacao> FindTransacaoByIdAsync(Guid transacaoId)
    {
        logger.LogInformation("Buscando uma Transacao");
        return await FindTransacaoOrThrowExceptionAsync(transacaoId);
    }

    public async Task<Transacao> UpdateTransacaoAsync(Transacao updateTransacaoRequest, Guid transacaoId)
    {
        logger.LogInformation("Editando uma Transacao");

        Transacao transacao = await FindTransacaoOrThrowExceptionAsync(transacaoId);

        transacao.Descricao = updateTransacaoRequest.Descricao;
        transacao.Valor = updateTransacaoRequest.Valor;
        transacao.Tipo = updateTransacaoRequest.Tipo;
        transacao.CategoriaId = updateTransacaoRequest.CategoriaId;
        transacao.PessoaId = updateTransacaoRequest.PessoaId;
        await repository.UpdateTransacaoAsync(transacao);

        return await FindTransacaoOrThrowExceptionAsync(transacaoId);
    }

    public async Task DeleteTransacaoAsync(Guid transacaoId)
    {
        await FindTransacaoOrThrowExceptionAsync(transacaoId);
        await repository.DeleteTransacaoAsync(transacaoId);
    }

    private async Task<Transacao> FindTransacaoOrThrowExceptionAsync(Guid transacaoId)
    {
        Transacao transacao = await repository.FindTransacaoByIdAsync(transacaoId);

        if (transacao == null)
        {
            logger.LogInformation("Transacao nao encontrada");
            throw new NotFoundException("Transação não encontrada");
        }

        return transacao;
    }
}
