using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

public interface IPessoaService
{
    Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
    Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas);
    Task<Pessoa> FindPessoaByIdAsync(decimal pessoaId);
    Task<Pessoa> UpdatePessoaAsync(Pessoa updatePessoaRequest, decimal pessoaId);
    Task DeletePessoaAsync(decimal pessoaId);
}
