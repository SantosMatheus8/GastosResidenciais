using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;

namespace ResiGa.Bkd.Service;

public class PessoaService(IPessoaRepository repository, ILogger<PessoaService> logger) : IPessoaService
{
    public async Task<Pessoa> CreatePessoaAsync(Pessoa pessoa)
    {
        logger.LogInformation("Criando Pessoa");

        return await repository.CreatePessoaAsync(pessoa);
    }

    public async Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas)
    {
        logger.LogInformation("Listando Pessoa");
        return await repository.GetPessoasAsync(listPessoas);
    }

    public async Task<Pessoa> FindPessoaByIdAsync(decimal pessoaId)
    {
        logger.LogInformation("Buscando um Pessoa");
        return await FindPessoaOrThrowExceptionAsync(pessoaId);
    }

    public async Task<Pessoa> UpdatePessoaAsync(Pessoa updatePessoaRequest, decimal pessoaId)
    {
        logger.LogInformation("Editando um Pessoa");

        Pessoa pessoa = await FindPessoaOrThrowExceptionAsync(pessoaId);

        pessoa.Nome = updatePessoaRequest.Nome;
        pessoa.Idade = updatePessoaRequest.Idade;
        await repository.UpdatePessoaAsync(pessoa);

        return await FindPessoaOrThrowExceptionAsync(pessoaId);
    }

    public async Task DeletePessoaAsync(decimal pessoaId)
    {
        await FindPessoaOrThrowExceptionAsync(pessoaId);
        await repository.DeletePessoaAsync(pessoaId);
    }

    private async Task<Pessoa> FindPessoaOrThrowExceptionAsync(decimal pessoaId)
    {
        Pessoa pessoa = await repository.FindPessoaByIdAsync(pessoaId);

        if (pessoa == null)
        {
            logger.LogInformation("Pessoa nao encontrado");
            throw new NotFoundException("Pessoa não encontrado");
        }

        return pessoa;
    }
}
