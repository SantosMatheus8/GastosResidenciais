using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Pessoa;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using Mapster;
using ResiGa.Bkd.Domain.Models.Pessoa;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ResiGa.Bkd.Api.Controllers;

[Route("v1/[controller]")]
[ApiController]
[Authorize]
public class PessoaController(IPessoaService pessoaService) : ControllerBase
{
    /// <summary>
    ///     Atraves dessa rota voce sera capaz de criar um pessoa
    /// </summary>
    /// <param name="createPessoaRequest">O objeto de requisicao para criar um pessoa</param>
    /// <returns>O pessoa criado</returns>
    /// <response code="201">Sucesso, e retorna um pessoa</response>
    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponse>> CreatePessoa([FromBody] CreatePessoaRequest createPessoaRequest)
    {       
        var pessoa = createPessoaRequest.Adapt<Pessoa>();
        Pessoa pessoaCreated = await pessoaService.CreatePessoaAsync(pessoa);
        var pessoaResponse = pessoaCreated.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar uma lista paginada de pessoas
    /// </summary>
    /// <param name="listPessoasRequest">O objeto de requisicao para buscar a lista paginada de pessoas</param>
    /// <returns>Uma lista paginada de pessoas</returns>
    /// <response code="200">Sucesso, e retorna uma lista paginada de pessoas</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PessoaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResult<PessoaResponse>>> GetPessoas([FromQuery] ListPessoasRequest listPessoasRequest)
    {
        var listPessoas = listPessoasRequest.Adapt<ListPessoas>();
        PaginatedResult<Pessoa> pessoas = await pessoaService.GetPessoasAsync(listPessoas);
        var pessoasResponse = pessoas.Adapt<PaginatedResult<PessoaResponse>>();
        return Ok(pessoasResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar um Pessoa
    /// </summary>
    /// <param name="pessoaId">O codigo Pessoa</param>
    /// <returns>O Pessoa consultado</returns>
    /// <response code="200">Sucesso, e retorna um Pessoa</response>
    [HttpGet("{pessoaId:decimal}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponse>> FindPessoaById([FromRoute] decimal pessoaId)
    {
        Pessoa pessoa = await pessoaService.FindPessoaByIdAsync(pessoaId);
        var pessoaResponse = pessoa.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    [HttpPut("{pessoaId:decimal}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponse>> UpdatePessoa([FromBody] UpdatePessoaRequest updatePessoaRequest,
        [FromRoute] decimal pessoaId)
    {
        var pessoa = updatePessoaRequest.Adapt<Pessoa>();
        Pessoa updatedPessoa = await pessoaService.UpdatePessoaAsync(pessoa, pessoaId);
        var pessoaResponse = updatedPessoa.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de deletar um pessoa
    /// </summary>
    /// <param name="pessoaId">O codigo do pessoa</param>
    /// <returns>Confirmação de deleção</returns>
    /// <response code="204">Sucesso, e retorna confirmação de deleção</response>
    [HttpDelete("{pessoaId:decimal}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePessoa([FromRoute] decimal pessoaId)
    {
        await pessoaService.DeletePessoaAsync(pessoaId);
        return NoContent();
    }
}
