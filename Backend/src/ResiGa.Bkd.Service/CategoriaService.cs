using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Service;


public class CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger) : ICategoriaService
{
    private const int DescricaoMaxLength = 400;

    public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
    {
        logger.LogInformation("Criando Categoria");

        ValidarCategoria(categoria);

        return await repository.CreateCategoriaAsync(categoria);
    }

    public async Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias)
    {
        logger.LogInformation("Listando Categoria");
        return await repository.GetCategoriasAsync(listCategorias);
    }

    public async Task<Categoria> FindCategoriaByIdAsync(Guid categoriaId)
    {
        logger.LogInformation("Buscando uma Categoria");
        return await FindCategoriaOrThrowExceptionAsync(categoriaId);
    }

    public async Task<Categoria> UpdateCategoriaAsync(Categoria updateCategoriaRequest, Guid categoriaId)
    {
        logger.LogInformation("Editando uma Categoria");

        ValidarCategoria(updateCategoriaRequest);

        Categoria categoria = await FindCategoriaOrThrowExceptionAsync(categoriaId);

        categoria.Descricao = updateCategoriaRequest.Descricao;
        categoria.Finalidade = updateCategoriaRequest.Finalidade;
        await repository.UpdateCategoriaAsync(categoria);

        return await FindCategoriaOrThrowExceptionAsync(categoriaId);
    }

    public async Task DeleteCategoriaAsync(Guid categoriaId)
    {
        await FindCategoriaOrThrowExceptionAsync(categoriaId);
        await repository.DeleteCategoriaAsync(categoriaId);
    }

    public async Task<RelatorioTotais<TotalPorCategoria>> GetTotaisPorCategoriaAsync()
    {
        logger.LogInformation("Buscando totais por categoria");

        var totais = await repository.GetTotaisPorCategoriaAsync();

        return new RelatorioTotais<TotalPorCategoria>
        {
            Itens = totais,
            TotalGeralReceitas = totais.Sum(t => t.TotalReceitas),
            TotalGeralDespesas = totais.Sum(t => t.TotalDespesas),
            SaldoLiquido = totais.Sum(t => t.TotalReceitas) - totais.Sum(t => t.TotalDespesas)
        };
    }

    private async Task<Categoria> FindCategoriaOrThrowExceptionAsync(Guid categoriaId)
    {
        Categoria categoria = await repository.FindCategoriaByIdAsync(categoriaId);

        if (categoria == null)
        {
            logger.LogInformation("Categoria nao encontrada");
            throw new NotFoundException("Categoria não encontrada");
        }

        return categoria;
    }

    private static void ValidarCategoria(Categoria categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria.Descricao))
            throw new UnprocessableEntityException("A descrição da categoria é obrigatória");

        if (categoria.Descricao.Length > DescricaoMaxLength)
            throw new UnprocessableEntityException($"A descrição da categoria deve ter no máximo {DescricaoMaxLength} caracteres");

        if (categoria.Finalidade < 0 || categoria.Finalidade > 2)
            throw new UnprocessableEntityException("A finalidade da categoria deve ser: 0 (Despesa), 1 (Receita) ou 2 (Ambas)");
    }
}


