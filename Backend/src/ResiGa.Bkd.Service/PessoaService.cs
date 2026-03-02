using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Service;

/// <summary>
/// Servico responsavel pelas regras de negocio da entidade Pessoa.
/// Valida campos obrigatorios, tamanho maximo e delega operacoes ao repositorio.
/// </summary>
public class PessoaService(IPessoaRepository repository, ILogger<PessoaService> logger) : IPessoaService
{
    /// <summary>
    /// Tamanho maximo permitido para o campo Nome (200 caracteres).
    /// </summary>
    private const int NomeMaxLength = 200;

    /// <summary>
    /// Cria uma nova pessoa apos validar os campos obrigatorios.
    /// Validacoes: Nome nao pode ser vazio e deve ter no maximo 200 caracteres.
    /// </summary>
    public async Task<Pessoa> CreatePessoaAsync(Pessoa pessoa)
    {
        logger.LogInformation("Criando Pessoa");

        ValidarPessoa(pessoa);

        return await repository.CreatePessoaAsync(pessoa);
    }

    /// <summary>
    /// Lista pessoas com suporte a filtros e paginacao.
    /// </summary>
    public async Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas)
    {
        logger.LogInformation("Listando Pessoa");
        return await repository.GetPessoasAsync(listPessoas);
    }

    /// <summary>
    /// Busca uma pessoa pelo Id. Lanca NotFoundException se nao encontrada.
    /// </summary>
    public async Task<Pessoa> FindPessoaByIdAsync(Guid pessoaId)
    {
        logger.LogInformation("Buscando um Pessoa");
        return await FindPessoaOrThrowExceptionAsync(pessoaId);
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa existente apos validar os novos valores.
    /// Validacoes: Nome nao pode ser vazio e deve ter no maximo 200 caracteres.
    /// </summary>
    public async Task<Pessoa> UpdatePessoaAsync(Pessoa updatePessoaRequest, Guid pessoaId)
    {
        logger.LogInformation("Editando um Pessoa");

        ValidarPessoa(updatePessoaRequest);

        Pessoa pessoa = await FindPessoaOrThrowExceptionAsync(pessoaId);

        pessoa.Nome = updatePessoaRequest.Nome;
        pessoa.Idade = updatePessoaRequest.Idade;
        await repository.UpdatePessoaAsync(pessoa);

        return await FindPessoaOrThrowExceptionAsync(pessoaId);
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transacoes em cascata.
    /// REGRA DE NEGOCIO: ao deletar uma pessoa, todas as suas transacoes sao removidas automaticamente.
    /// A delecao em cascata e executada no repositorio dentro de uma unica transacao SQL.
    /// </summary>
    public async Task DeletePessoaAsync(Guid pessoaId)
    {
        await FindPessoaOrThrowExceptionAsync(pessoaId);
        await repository.DeletePessoaAsync(pessoaId);
    }

    /// <summary>
    /// Retorna relatorio com totais financeiros por pessoa e totais gerais.
    /// Calcula: total receitas, total despesas e saldo para cada pessoa,
    /// alem dos totais gerais somados de todas as pessoas.
    /// </summary>
    public async Task<RelatorioTotais<TotalPorPessoa>> GetTotaisPorPessoaAsync()
    {
        logger.LogInformation("Buscando totais por pessoa");

        var totais = await repository.GetTotaisPorPessoaAsync();

        return new RelatorioTotais<TotalPorPessoa>
        {
            Itens = totais,
            TotalGeralReceitas = totais.Sum(t => t.TotalReceitas),
            TotalGeralDespesas = totais.Sum(t => t.TotalDespesas),
            SaldoLiquido = totais.Sum(t => t.TotalReceitas) - totais.Sum(t => t.TotalDespesas)
        };
    }

    /// <summary>
    /// Busca pessoa por Id ou lanca NotFoundException (404).
    /// Metodo auxiliar reutilizado em operacoes que exigem que a pessoa exista.
    /// </summary>
    private async Task<Pessoa> FindPessoaOrThrowExceptionAsync(Guid pessoaId)
    {
        Pessoa pessoa = await repository.FindPessoaByIdAsync(pessoaId);

        if (pessoa == null)
        {
            logger.LogInformation("Pessoa nao encontrado");
            throw new NotFoundException("Pessoa não encontrada");
        }

        return pessoa;
    }

    /// <summary>
    /// Valida os campos da pessoa conforme regras de negocio:
    /// - Nome: obrigatorio, maximo 200 caracteres.
    /// - Idade: deve ser um valor nao negativo.
    /// Lanca UnprocessableEntityException (422) em caso de violacao.
    /// </summary>
    private static void ValidarPessoa(Pessoa pessoa)
    {
        if (string.IsNullOrWhiteSpace(pessoa.Nome))
            throw new UnprocessableEntityException("O nome da pessoa é obrigatório");

        if (pessoa.Nome.Length > NomeMaxLength)
            throw new UnprocessableEntityException($"O nome da pessoa deve ter no máximo {NomeMaxLength} caracteres");

        if (pessoa.Idade < 0)
            throw new UnprocessableEntityException("A idade da pessoa não pode ser negativa");
    }
}
