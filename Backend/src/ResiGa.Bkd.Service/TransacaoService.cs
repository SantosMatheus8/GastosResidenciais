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

/// <summary>
/// Servico responsavel pelas regras de negocio da entidade Transacao.
/// Aplica validacoes de campo, regra de menor de idade e compatibilidade de categoria.
/// Depende dos repositorios de Pessoa e Categoria para validar as regras de negocio.
/// </summary>
public class TransacaoService(
    ITransacaoRepository repository,
    IPessoaRepository pessoaRepository,
    ICategoriaRepository categoriaRepository,
    ILogger<TransacaoService> logger) : ITransacaoService
{
    /// <summary>
    /// Tamanho maximo permitido para o campo Descricao (400 caracteres).
    /// </summary>
    private const int DescricaoMaxLength = 400;

    /// <summary>
    /// Cria uma nova transacao apos aplicar todas as validacoes e regras de negocio.
    ///
    /// Validacoes aplicadas:
    /// 1. Descricao obrigatoria, max 400 caracteres
    /// 2. Valor deve ser positivo (maior que zero)
    /// 3. Tipo deve ser 0 (Despesa) ou 1 (Receita)
    /// 4. Pessoa e Categoria devem existir no banco
    /// 5. REGRA: Pessoa menor de 18 anos so pode ter transacoes do tipo Despesa
    /// 6. REGRA: A Categoria deve ser compativel com o Tipo da transacao
    /// </summary>
    public async Task<Transacao> CreateTransacaoAsync(Transacao transacao)
    {
        logger.LogInformation("Criando Transacao");
        ValidarTransacao(transacao);

        await ValidarRegrasDeNegocioAsync(transacao);

        return await repository.CreateTransacaoAsync(transacao);
    }

    /// <summary>
    /// Lista transacoes com suporte a filtros e paginacao.
    /// </summary>
    public async Task<PaginatedResult<Transacao>> GetTransacoesAsync(ListTransacoes listTransacoes)
    {
        logger.LogInformation("Listando Transacao");
        return await repository.GetTransacoesAsync(listTransacoes);
    }

    /// <summary>
    /// Busca uma transacao pelo Id. Lanca NotFoundException se nao encontrada.
    /// </summary>
    public async Task<Transacao> FindTransacaoByIdAsync(Guid transacaoId)
    {
        logger.LogInformation("Buscando uma Transacao");
        return await FindTransacaoOrThrowExceptionAsync(transacaoId);
    }

    /// <summary>
    /// Atualiza uma transacao existente apos reaplicar todas as validacoes e regras de negocio.
    /// </summary>
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

    /// <summary>
    /// Deleta uma transacao. Verifica se existe antes de deletar.
    /// </summary>
    public async Task DeleteTransacaoAsync(Guid transacaoId)
    {
        await FindTransacaoOrThrowExceptionAsync(transacaoId);
        await repository.DeleteTransacaoAsync(transacaoId);
    }

    /// <summary>
    /// Busca transacao por Id ou lanca NotFoundException (404).
    /// </summary>
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

    /// <summary>
    /// Valida os campos basicos da transacao:
    /// - Descricao: obrigatoria, maximo 400 caracteres
    /// - Valor: deve ser positivo (maior que zero)
    /// - Tipo: deve ser 0 (Despesa) ou 1 (Receita)
    /// Lanca UnprocessableEntityException (422) em caso de violacao.
    /// </summary>
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

    /// <summary>
    /// Valida as regras de negocio que dependem de dados do banco:
    ///
    /// REGRA 1 - Menor de idade:
    ///   Se a pessoa associada tiver menos de 18 anos, apenas transacoes do tipo Despesa (0) sao permitidas.
    ///   Isso impede que menores de idade registrem receitas no sistema.
    ///
    /// REGRA 2 - Compatibilidade Categoria x Tipo:
    ///   A categoria selecionada deve ser compativel com o tipo da transacao:
    ///   - Tipo = Despesa (0) → Categoria deve ter Finalidade = Despesa (0) ou Ambas (2)
    ///   - Tipo = Receita (1) → Categoria deve ter Finalidade = Receita (1) ou Ambas (2)
    /// </summary>
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
