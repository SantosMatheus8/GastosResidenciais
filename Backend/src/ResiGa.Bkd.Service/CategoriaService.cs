using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Service;

public class CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger) : ICategoriaService
{
    public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
    {
        logger.LogInformation("Criando Categoria");

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
}
