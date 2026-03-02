using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

/// <summary>
/// Interface do servico de Pessoa.
/// Define o contrato de regras de negocio para a entidade Pessoa.
/// </summary>
public interface IPessoaService
{
    Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
    Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas);
    Task<Pessoa> FindPessoaByIdAsync(Guid pessoaId);
    Task<Pessoa> UpdatePessoaAsync(Pessoa updatePessoaRequest, Guid pessoaId);
    Task DeletePessoaAsync(Guid pessoaId);

    /// <summary>
    /// Retorna relatorio com totais de receitas, despesas e saldo por pessoa,
    /// alem dos totais gerais somados de todas as pessoas.
    /// </summary>
    Task<RelatorioTotais<TotalPorPessoa>> GetTotaisPorPessoaAsync();
}
