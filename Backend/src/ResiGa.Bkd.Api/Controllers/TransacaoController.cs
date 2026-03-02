using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Transacao;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using Mapster;
using ResiGa.Bkd.Domain.Models.Transacao;

namespace ResiGa.Bkd.Api.Controllers;

/// <summary>
/// Controller responsavel pelos endpoints REST da entidade Transacao.
/// Aplica regras de negocio: menor de idade e compatibilidade de categoria.
/// </summary>
[Route("v1/[controller]")]
[ApiController]
public class TransacaoController(ITransacaoService transacaoService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova transacao no sistema.
    /// Validacoes aplicadas:
    /// - Descricao obrigatoria (max 400 chars)
    /// - Valor deve ser positivo
    /// - Tipo deve ser 0 (Despesa) ou 1 (Receita)
    /// - REGRA: Pessoa menor de 18 anos so pode ter Despesas
    /// - REGRA: Categoria deve ser compativel com o Tipo da transacao
    /// </summary>
    /// <param name="createTransacaoRequest">Dados da transacao a ser criada</param>
    /// <returns>A transacao criada com Id gerado</returns>
    /// <response code="200">Sucesso, retorna a transacao criada</response>
    /// <response code="404">Pessoa ou Categoria nao encontrada</response>
    /// <response code="422">Erro de validacao ou violacao de regra de negocio</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransacaoResponse>> CreateTransacao([FromBody] CreateTransacaoRequest createTransacaoRequest)
    {
        var transacao = createTransacaoRequest.Adapt<Transacao>();
        Transacao transacaoCreated = await transacaoService.CreateTransacaoAsync(transacao);
        var transacaoResponse = transacaoCreated.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    /// <summary>
    /// Lista transacoes com suporte a filtros e paginacao.
    /// Permite filtrar por Descricao, Tipo, CategoriaId e PessoaId.
    /// </summary>
    /// <param name="listTransacoesRequest">Filtros e parametros de paginacao</param>
    /// <returns>Lista paginada de transacoes</returns>
    /// <response code="200">Sucesso, retorna lista paginada</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<TransacaoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<TransacaoResponse>>> GetTransacoes([FromQuery] ListTransacoesRequest listTransacoesRequest)
    {
        var listTransacoes = listTransacoesRequest.Adapt<ListTransacoes>();
        PaginatedResult<Transacao> transacoes = await transacaoService.GetTransacoesAsync(listTransacoes);
        var transacoesResponse = transacoes.Adapt<PaginatedResult<TransacaoResponse>>();
        return Ok(transacoesResponse);
    }

    /// <summary>
    /// Busca uma transacao pelo seu Id unico.
    /// </summary>
    /// <param name="transacaoId">Id da transacao</param>
    /// <returns>Dados da transacao</returns>
    /// <response code="200">Sucesso, retorna a transacao</response>
    /// <response code="404">Transacao nao encontrada</response>
    [HttpGet("{transacaoId:guid}")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransacaoResponse>> FindTransacaoById([FromRoute] Guid transacaoId)
    {
        Transacao transacao = await transacaoService.FindTransacaoByIdAsync(transacaoId);
        var transacaoResponse = transacao.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    /// <summary>
    /// Atualiza os dados de uma transacao existente.
    /// Reaplica todas as validacoes e regras de negocio na atualizacao.
    /// </summary>
    /// <param name="updateTransacaoRequest">Novos dados da transacao</param>
    /// <param name="transacaoId">Id da transacao a ser atualizada</param>
    /// <returns>Transacao atualizada</returns>
    /// <response code="200">Sucesso, retorna a transacao atualizada</response>
    /// <response code="404">Transacao, Pessoa ou Categoria nao encontrada</response>
    /// <response code="422">Erro de validacao ou violacao de regra de negocio</response>
    [HttpPut("{transacaoId:guid}")]
    [ProducesResponseType(typeof(TransacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<TransacaoResponse>> UpdateTransacao([FromBody] UpdateTransacaoRequest updateTransacaoRequest,
        [FromRoute] Guid transacaoId)
    {
        var transacao = updateTransacaoRequest.Adapt<Transacao>();
        Transacao updatedTransacao = await transacaoService.UpdateTransacaoAsync(transacao, transacaoId);
        var transacaoResponse = updatedTransacao.Adapt<TransacaoResponse>();
        return Ok(transacaoResponse);
    }

    /// <summary>
    /// Deleta uma transacao pelo Id.
    /// </summary>
    /// <param name="transacaoId">Id da transacao a ser deletada</param>
    /// <returns>Confirmacao de delecao (sem conteudo)</returns>
    /// <response code="204">Sucesso, transacao deletada</response>
    /// <response code="404">Transacao nao encontrada</response>
    [HttpDelete("{transacaoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTransacao([FromRoute] Guid transacaoId)
    {
        await transacaoService.DeleteTransacaoAsync(transacaoId);
        return NoContent();
    }
}
