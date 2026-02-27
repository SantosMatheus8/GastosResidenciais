using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;

namespace ResiGa.Bkd.Domain.Interfaces.Services;

public interface IPessoaService
{
    Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
    Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas);
    Task<Pessoa> FindPessoaByIdAsync(Guid pessoaId);
    Task<Pessoa> UpdatePessoaAsync(Pessoa updatePessoaRequest, Guid pessoaId);
    Task DeletePessoaAsync(Guid pessoaId);
}
