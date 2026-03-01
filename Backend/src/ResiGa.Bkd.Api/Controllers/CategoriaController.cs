using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Categoria;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using Mapster;
using ResiGa.Bkd.Domain.Models.Categoria;

namespace ResiGa.Bkd.Api.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class CategoriaController(ICategoriaService categoriaService) : ControllerBase
{
    /// <summary>
    ///     Atraves dessa rota voce sera capaz de criar uma categoria
    /// </summary>
    /// <param name="createCategoriaRequest">O objeto de requisicao para criar uma categoria</param>
    /// <returns>A categoria criada</returns>
    /// <response code="201">Sucesso, e retorna uma categoria</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> CreateCategoria([FromBody] CreateCategoriaRequest createCategoriaRequest)
    {
        var categoria = createCategoriaRequest.Adapt<Categoria>();
        Categoria categoriaCreated = await categoriaService.CreateCategoriaAsync(categoria);
        var categoriaResponse = categoriaCreated.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar uma lista paginada de categorias
    /// </summary>
    /// <param name="listCategoriasRequest">O objeto de requisicao para buscar a lista paginada de categorias</param>
    /// <returns>Uma lista paginada de categorias</returns>
    /// <response code="200">Sucesso, e retorna uma lista paginada de categorias</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CategoriaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResult<CategoriaResponse>>> GetCategorias([FromQuery] ListCategoriasRequest listCategoriasRequest)
    {
        var listCategorias = listCategoriasRequest.Adapt<ListCategorias>();
        PaginatedResult<Categoria> categorias = await categoriaService.GetCategoriasAsync(listCategorias);
        var categoriasResponse = categorias.Adapt<PaginatedResult<CategoriaResponse>>();
        return Ok(categoriasResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar uma Categoria
    /// </summary>
    /// <param name="categoriaId">O codigo da Categoria</param>
    /// <returns>A Categoria consultada</returns>
    /// <response code="200">Sucesso, e retorna uma Categoria</response>
    [HttpGet("{categoriaId:guid}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> FindCategoriaById([FromRoute] Guid categoriaId)
    {
        Categoria categoria = await categoriaService.FindCategoriaByIdAsync(categoriaId);
        var categoriaResponse = categoria.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    [HttpPut("{categoriaId:guid}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaResponse>> UpdateCategoria([FromBody] UpdateCategoriaRequest updateCategoriaRequest,
        [FromRoute] Guid categoriaId)
    {
        var categoria = updateCategoriaRequest.Adapt<Categoria>();
        Categoria updatedCategoria = await categoriaService.UpdateCategoriaAsync(categoria, categoriaId);
        var categoriaResponse = updatedCategoria.Adapt<CategoriaResponse>();
        return Ok(categoriaResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de deletar uma categoria
    /// </summary>
    /// <param name="categoriaId">O codigo da categoria</param>
    /// <returns>Confirmação de deleção</returns>
    /// <response code="204">Sucesso, e retorna confirmação de deleção</response>
    [HttpDelete("{categoriaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategoria([FromRoute] Guid categoriaId)
    {
        await categoriaService.DeleteCategoriaAsync(categoriaId);
        return NoContent();
    }
}
