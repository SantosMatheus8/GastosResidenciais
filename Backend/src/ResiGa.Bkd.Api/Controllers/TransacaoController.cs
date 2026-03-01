using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Transacao;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using Mapster;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Api.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class TransacaoController(ITransacaoService transacaoService) : ControllerBase
{
    /// <summary>
    ///     Atraves dessa rota voce sera capaz de criar uma transacao
    /// </summary>
    /// <param name="createTransacaoRequest">O objeto de requisicao para criar uma transacao</param>
    /// <returns>A transacao criada</returns>
    /// <response code="201">Sucesso, e retorna uma transacao</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransacaoResponse>> CreateTransacao([FromBody] CreateTransacaoRequest createTransacaoRequest)
    {
        var transacao = createTransacaoRequest.Adapt<Transacao>();
        Transacao transacaoCreated = await transacaoService.CreateTransacaoAsync(transacao);
        var transacaoResponse = transacaoCreated.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar uma lista paginada de transacoes
    /// </summary>
    /// <param name="listTransacoesRequest">O objeto de requisicao para buscar a lista paginada de transacoes</param>
    /// <returns>Uma lista paginada de transacoes</returns>
    /// <response code="200">Sucesso, e retorna uma lista paginada de transacoes</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<TransacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResult<TransacaoResponse>>> GetTransacoes([FromQuery] ListTransacoesRequest listTransacoesRequest)
    {
        var listTransacoes = listTransacoesRequest.Adapt<ListTransacoes>();
        PaginatedResult<Transacao> transacoes = await transacaoService.GetTransacoesAsync(listTransacoes);
        var transacoesResponse = transacoes.Adapt<PaginatedResult<TransacaoResponse>>();
        return Ok(transacoesResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de buscar uma Transacao
    /// </summary>
    /// <param name="transacaoId">O codigo da Transacao</param>
    /// <returns>A Transacao consultada</returns>
    /// <response code="200">Sucesso, e retorna uma Transacao</response>
    [HttpGet("{transacaoId:guid}")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransacaoResponse>> FindTransacaoById([FromRoute] Guid transacaoId)
    {
        Transacao transacao = await transacaoService.FindTransacaoByIdAsync(transacaoId);
        var transacaoResponse = transacao.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    [HttpPut("{transacaoId:guid}")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransacaoResponse>> UpdateTransacao([FromBody] UpdateTransacaoRequest updateTransacaoRequest,
        [FromRoute] Guid transacaoId)
    {
        var transacao = updateTransacaoRequest.Adapt<Transacao>();
        Transacao updatedTransacao = await transacaoService.UpdateTransacaoAsync(transacao, transacaoId);
        var transacaoResponse = updatedTransacao.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    /// <summary>
    ///     Atraves dessa rota voce sera capaz de deletar uma transacao
    /// </summary>
    /// <param name="transacaoId">O codigo da transacao</param>
    /// <returns>Confirmação de deleção</returns>
    /// <response code="204">Sucesso, e retorna confirmação de deleção</response>
    [HttpDelete("{transacaoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTransacao([FromRoute] Guid transacaoId)
    {
        await transacaoService.DeleteTransacaoAsync(transacaoId);
        return NoContent();
    }
}
