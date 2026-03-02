using Microsoft.Extensions.Logging;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Domain.Interfaces.Repositories;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Categoria;
using ResiGa.Bkd.Domain.Models.Relatorios;

namespace ResiGa.Bkd.Service;

/// <summary>
/// Servico responsavel pelas regras de negocio da entidade Categoria.
/// Valida campos obrigatorios, tamanho maximo e valores de enum.
/// </summary>
public class CategoriaService(ICategoriaRepository repository, ILogger<CategoriaService> logger) : ICategoriaService
{
    /// <summary>
    /// Tamanho maximo permitido para o campo Descricao (400 caracteres).
    /// </summary>
    private const int DescricaoMaxLength = 400;

    /// <summary>
    /// Cria uma nova categoria apos validar os campos.
    /// Validacoes: Descricao obrigatoria (max 400 chars), Finalidade deve ser 0, 1 ou 2.
    /// </summary>
    public async Task<Categoria> CreateCategoriaAsync(Categoria categoria)
    {
        logger.LogInformation("Criando Categoria");

        ValidarCategoria(categoria);

        return await repository.CreateCategoriaAsync(categoria);
    }

    /// <summary>
    /// Lista categorias com suporte a filtros e paginacao.
    /// </summary>
    public async Task<PaginatedResult<Categoria>> GetCategoriasAsync(ListCategorias listCategorias)
    {
        logger.LogInformation("Listando Categoria");
        return await repository.GetCategoriasAsync(listCategorias);
    }

    /// <summary>
    /// Busca uma categoria pelo Id. Lanca NotFoundException se nao encontrada.
    /// </summary>
    public async Task<Categoria> FindCategoriaByIdAsync(Guid categoriaId)
    {
        logger.LogInformation("Buscando uma Categoria");
        return await FindCategoriaOrThrowExceptionAsync(categoriaId);
    }

    /// <summary>
    /// Atualiza os dados de uma categoria existente apos validar os novos valores.
    /// </summary>
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

    /// <summary>
    /// Deleta uma categoria. Verifica se existe antes de deletar.
    /// </summary>
    public async Task DeleteCategoriaAsync(Guid categoriaId)
    {
        await FindCategoriaOrThrowExceptionAsync(categoriaId);
        await repository.DeleteCategoriaAsync(categoriaId);
    }

    /// <summary>
    /// Retorna relatorio com totais financeiros por categoria e totais gerais.
    /// Calcula: total receitas, total despesas e saldo para cada categoria,
    /// alem dos totais gerais somados de todas as categorias.
    /// </summary>
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

    /// <summary>
    /// Busca categoria por Id ou lanca NotFoundException (404).
    /// </summary>
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

    /// <summary>
    /// Valida os campos da categoria conforme regras de negocio:
    /// - Descricao: obrigatoria, maximo 400 caracteres.
    /// - Finalidade: deve ser 0 (Despesa), 1 (Receita) ou 2 (Ambas).
    /// Lanca UnprocessableEntityException (422) em caso de violacao.
    /// </summary>
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


