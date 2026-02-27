using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;

namespace ResiGa.Bkd.Domain.Interfaces.Repositories;

public interface IPessoaRepository
{
    Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
    Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas);
    Task<Pessoa> FindPessoaByIdAsync(decimal pessoaId);
    Task UpdatePessoaAsync(Pessoa pessoa);
    Task DeletePessoaAsync(decimal pessoaId);
}