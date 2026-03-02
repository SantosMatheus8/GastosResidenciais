using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Categoria;
using ResiGa.Bkd.Api.Dtos.Relatorios;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Relatorios;
using Mapster;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Api.Controllers;

/// <summary>
/// Controller responsavel pelos endpoints REST da entidade Categoria.
/// Disponibiliza operacoes de CRUD e consulta de totais financeiros por categoria.
/// </summary>
[Route("v1/[controller]")]
[ApiController]
public class CategoriaController(ICategoriaService categoriaService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova categoria no sistema.
    /// Validacoes: Descricao obrigatoria (max 400 chars), Finalidade deve ser 0 (Despesa), 1 (Receita) ou 2 (Ambas).
    /// </summary>
    /// <param name="createCategoriaRequest">Dados da categoria a ser criada</param>
    /// <returns>A categoria criada com Id gerado</returns>
    /// <response code="200">Sucesso, retorna a categoria criada</response>
    /// <response code="422">Erro de validacao nos campos</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CategoriaResponse>> CreateCategoria([FromBody] CreateCategoriaRequest createCategoriaRequest)
    {
        var categoria = createCategoriaRequest.Adapt<Categoria>();
        Categoria categoriaCreated = await categoriaService.CreateCategoriaAsync(categoria);
        var categoriaResponse = categoriaCreated.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    /// <summary>
    /// Lista categorias com suporte a filtros e paginacao.
    /// Permite filtrar por Descricao (busca parcial) e Finalidade (busca exata).
    /// </summary>
    /// <param name="listCategoriasRequest">Filtros e parametros de paginacao</param>
    /// <returns>Lista paginada de categorias</returns>
    /// <response code="200">Sucesso, retorna lista paginada</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CategoriaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<CategoriaResponse>>> GetCategorias([FromQuery] ListCategoriasRequest listCategoriasRequest)
    {
        var listCategorias = listCategoriasRequest.Adapt<ListCategorias>();
        PaginatedResult<Categoria> categorias = await categoriaService.GetCategoriasAsync(listCategorias);
        var categoriasResponse = categorias.Adapt<PaginatedResult<CategoriaResponse>>();
        return Ok(categoriasResponse);
    }

    /// <summary>
    /// Busca uma categoria pelo seu Id unico.
    /// </summary>
    /// <param name="categoriaId">Id da categoria</param>
    /// <returns>Dados da categoria</returns>
    /// <response code="200">Sucesso, retorna a categoria</response>
    /// <response code="404">Categoria nao encontrada</response>
    [HttpGet("{categoriaId:guid}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> FindCategoriaById([FromRoute] Guid categoriaId)
    {
        Categoria categoria = await categoriaService.FindCategoriaByIdAsync(categoriaId);
        var categoriaResponse = categoria.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    /// <summary>
    /// Atualiza os dados de uma categoria existente.
    /// Validacoes: Descricao obrigatoria (max 400 chars), Finalidade valida.
    /// </summary>
    /// <param name="updateCategoriaRequest">Novos dados da categoria</param>
    /// <param name="categoriaId">Id da categoria a ser atualizada</param>
    /// <returns>Categoria atualizada</returns>
    /// <response code="200">Sucesso, retorna a categoria atualizada</response>
    /// <response code="404">Categoria nao encontrada</response>
    /// <response code="422">Erro de validacao nos campos</response>
    [HttpPut("{categoriaId:guid}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CategoriaResponse>> UpdateCategoria([FromBody] UpdateCategoriaRequest updateCategoriaRequest,
        [FromRoute] Guid categoriaId)
    {
        var categoria = updateCategoriaRequest.Adapt<Categoria>();
        Categoria updatedCategoria = await categoriaService.UpdateCategoriaAsync(categoria, categoriaId);
        var categoriaResponse = updatedCategoria.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    /// <summary>
    /// Deleta uma categoria pelo Id.
    /// </summary>
    /// <param name="categoriaId">Id da categoria a ser deletada</param>
    /// <returns>Confirmacao de delecao (sem conteudo)</returns>
    /// <response code="204">Sucesso, categoria deletada</response>
    /// <response code="404">Categoria nao encontrada</response>
    [HttpDelete("{categoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategoria([FromRoute] Guid categoriaId)
    {
        await categoriaService.DeleteCategoriaAsync(categoriaId);
        return NoContent();
    }

    /// <summary>
    /// Retorna relatorio de totais financeiros agrupados por categoria.
    /// Para cada categoria, exibe: total de receitas, total de despesas e saldo (receita - despesa).
    /// Ao final, exibe os totais gerais somados de todas as categorias.
    /// </summary>
    /// <returns>Relatorio com totais por categoria e totais gerais</returns>
    /// <response code="200">Sucesso, retorna o relatorio de totais</response>
    [HttpGet("totais")]
    [ProducesResponseType(typeof(RelatorioTotaisResponse<TotalPorCategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RelatorioTotaisResponse<TotalPorCategoriaResponse>>> GetTotaisPorCategoria()
    {
        var relatorio = await categoriaService.GetTotaisPorCategoriaAsync();
        var response = relatorio.Adapt<RelatorioTotaisResponse<TotalPorCategoriaResponse>>();
        return Ok(response);
    }
}
