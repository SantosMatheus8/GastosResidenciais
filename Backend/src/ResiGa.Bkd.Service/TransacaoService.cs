using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Enums;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Transacao;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Service;

public class TransacaoService(
    ITransacaoRepository repository,
    IPessoaRepository pessoaRepository,
    ICategoriaRepository categoriaRepository,
    ILogger<TransacaoService> logger) : ITransacaoService
{
    private const int DescricaoMaxLength = 400;

    public async Task<Transacao> CreateTransacaoAsync(Transacao transacao)
    {
        logger.LogInformation("Criando Transacao");
        ValidarTransacao(transacao);

        await ValidarRegrasDeNegocioAsync(transacao);

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
        ValidarTransacao(updateTransacaoRequest);

        await ValidarRegrasDeNegocioAsync(updateTransacaoRequest);

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

    private static void ValidarTransacao(Transacao transacao)
    {
        if (string.IsNullOrWhiteSpace(transacao.Descricao))
            throw new UnprocessableEntityException("A descrição da transação é obrigatória");

        if (transacao.Descricao.Length > DescricaoMaxLength)
            throw new UnprocessableEntityException($"A descrição da transação deve ter no máximo {DescricaoMaxLength} caracteres");

        if (transacao.Valor <= 0)
            throw new UnprocessableEntityException("O valor da transação deve ser positivo (maior que zero)");

        if (transacao.Tipo < 0 || transacao.Tipo > 1)
            throw new UnprocessableEntityException("O tipo da transação deve ser: 0 (Despesa) ou 1 (Receita)");
    }

    private async Task ValidarRegrasDeNegocioAsync(Transacao transacao)
    {
        Pessoa pessoa = await pessoaRepository.FindPessoaByIdAsync(transacao.PessoaId);
        if (pessoa == null)
            throw new NotFoundException("Pessoa não encontrada");

        if (pessoa.Idade < 18 && transacao.Tipo != (int)TipoTransacao.Despesa)
        {
            logger.LogWarning("Tentativa de criar receita para menor de idade. PessoaId: {PessoaId}, Idade: {Idade}",
                transacao.PessoaId, pessoa.Idade);
            throw new UnprocessableEntityException(
                "Pessoa menor de 18 anos só pode registrar transações do tipo Despesa");
        }

        Categoria categoria = await categoriaRepository.FindCategoriaByIdAsync(transacao.CategoriaId);
        if (categoria == null)
            throw new NotFoundException("Categoria não encontrada");

        var tipoTransacao = (TipoTransacao)transacao.Tipo;
        var finalidadeCategoria = (Finalidade)categoria.Finalidade;

        bool categoriaCompativel = finalidadeCategoria switch
        {
            Finalidade.Ambas => true,
            Finalidade.Despesa => tipoTransacao == TipoTransacao.Despesa,
            Finalidade.Receita => tipoTransacao == TipoTransacao.Receita,
            _ => false
        };

        if (!categoriaCompativel)
        {
            logger.LogWarning(
                "Incompatibilidade entre tipo de transacao ({Tipo}) e finalidade da categoria ({Finalidade}). CategoriaId: {CategoriaId}",
                tipoTransacao, finalidadeCategoria, transacao.CategoriaId);
            throw new UnprocessableEntityException(
                $"A categoria selecionada (Finalidade: {finalidadeCategoria}) não é compatível com o tipo de transação ({tipoTransacao})");
        }
    }
}
