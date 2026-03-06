using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Pessoa;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Service;

public class PessoaService(IPessoaRepository repository, ILogger<PessoaService> logger) : IPessoaService
{
    private const int NomeMaxLength = 200;

    public async Task<Pessoa> CreatePessoaAsync(Pessoa pessoa)
    {
        logger.LogInformation("Criando Pessoa");

        ValidarPessoa(pessoa);

        return await repository.CreatePessoaAsync(pessoa);
    }

    public async Task<PaginatedResult<Pessoa>> GetPessoasAsync(ListPessoas listPessoas)
    {
        logger.LogInformation("Listando Pessoa");
        return await repository.GetPessoasAsync(listPessoas);
    }

    public async Task<Pessoa> FindPessoaByIdAsync(Guid pessoaId)
    {
        logger.LogInformation("Buscando um Pessoa");
        return await FindPessoaOrThrowExceptionAsync(pessoaId);
    }

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

    public async Task DeletePessoaAsync(Guid pessoaId)
    {
        await FindPessoaOrThrowExceptionAsync(pessoaId);
        await repository.DeletePessoaAsync(pessoaId);
    }

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
