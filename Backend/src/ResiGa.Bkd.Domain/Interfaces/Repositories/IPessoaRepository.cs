using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Domain.Interfaces.Repositories;

/// <summary>
/// Interface do repositorio de Pessoa.
/// Define o contrato de acesso a dados para operacoes com a entidade Pessoa.
/// </summary>
public interface IPessoaRepository
{
    Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
    Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas);
    Task<Pessoa?> FindPessoaByIdAsync(Guid pessoaId);
    Task UpdatePessoaAsync(Pessoa pessoa);
    Task DeletePessoaAsync(Guid pessoaId);

    /// <summary>
    /// Retorna os totais financeiros (receitas, despesas, saldo) agrupados por pessoa.
    /// </summary>
    Task<List<TotalPorPessoa>> GetTotaisPorPessoaAsync();
}
