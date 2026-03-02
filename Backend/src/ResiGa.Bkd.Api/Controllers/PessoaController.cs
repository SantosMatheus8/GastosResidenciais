using Microsoft.AspNetCore.Mvc;
using ResiGa.Bkd.Api.Dtos.Pessoa;
using ResiGa.Bkd.Api.Dtos.Relatorios;
using ResiGa.Bkd.Domain.Interfaces.Services;
using ResiGa.Bkd.Domain.Models;
using ResiGa.Bkd.Domain.Models.Relatorios;
using Mapster;
using ResiGa.Bkd.Domain.Models.Pessoa;

namespace ResiGa.Bkd.Api.Controllers;

/// <summary>
/// Controller responsavel pelos endpoints REST da entidade Pessoa.
/// Disponibiliza operacoes de CRUD completo e consulta de totais financeiros.
/// </summary>
[Route("v1/[controller]")]
[ApiController]
public class PessoaController(IPessoaService pessoaService) : ControllerBase
{
    /// <summary>
    /// Cria uma nova pessoa no sistema.
    /// Validacoes: Nome obrigatorio (max 200 chars), Idade nao negativa.
    /// </summary>
    /// <param name="createPessoaRequest">Dados da pessoa a ser criada</param>
    /// <returns>A pessoa criada com Id gerado</returns>
    /// <response code="200">Sucesso, retorna a pessoa criada</response>
    /// <response code="422">Erro de validacao nos campos</response>
    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PessoaResponse>> CreatePessoa([FromBody] CreatePessoaRequest createPessoaRequest)
    {
        var pessoa = createPessoaRequest.Adapt<Pessoa>();
        Pessoa pessoaCreated = await pessoaService.CreatePessoaAsync(pessoa);
        var pessoaResponse = pessoaCreated.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    /// <summary>
    /// Lista pessoas com suporte a filtros e paginacao.
    /// Permite filtrar por Nome (busca parcial) e Idade (busca exata).
    /// </summary>
    /// <param name="listPessoasRequest">Filtros e parametros de paginacao</param>
    /// <returns>Lista paginada de pessoas</returns>
    /// <response code="200">Sucesso, retorna lista paginada</response>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PessoaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<PessoaResponse>>> GetPessoas([FromQuery] ListPessoasRequest listPessoasRequest)
    {
        var listPessoas = listPessoasRequest.Adapt<ListPessoas>();
        PaginatedResult<Pessoa> pessoas = await pessoaService.GetPessoasAsync(listPessoas);
        var pessoasResponse = pessoas.Adapt<PaginatedResult<PessoaResponse>>();
        return Ok(pessoasResponse);
    }

    /// <summary>
    /// Busca uma pessoa pelo seu Id unico.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa</param>
    /// <returns>Dados da pessoa</returns>
    /// <response code="200">Sucesso, retorna a pessoa</response>
    /// <response code="404">Pessoa nao encontrada</response>
    [HttpGet("{pessoaId:guid}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponse>> FindPessoaById([FromRoute] Guid pessoaId)
    {
        Pessoa pessoa = await pessoaService.FindPessoaByIdAsync(pessoaId);
        var pessoaResponse = pessoa.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    /// <summary>
    /// Atualiza os dados de uma pessoa existente.
    /// Validacoes: Nome obrigatorio (max 200 chars), Idade nao negativa.
    /// </summary>
    /// <param name="updatePessoaRequest">Novos dados da pessoa</param>
    /// <param name="pessoaId">Id da pessoa a ser atualizada</param>
    /// <returns>Pessoa atualizada</returns>
    /// <response code="200">Sucesso, retorna a pessoa atualizada</response>
    /// <response code="404">Pessoa nao encontrada</response>
    /// <response code="422">Erro de validacao nos campos</response>
    [HttpPut("{pessoaId:guid}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<PessoaResponse>> UpdatePessoa([FromBody] UpdatePessoaRequest updatePessoaRequest,
        [FromRoute] Guid pessoaId)
    {
        var pessoa = updatePessoaRequest.Adapt<Pessoa>();
        Pessoa updatedPessoa = await pessoaService.UpdatePessoaAsync(pessoa, pessoaId);
        var pessoaResponse = updatedPessoa.Adapt<PessoaResponse>();
        return Ok(pessoaResponse);
    }

    /// <summary>
    /// Deleta uma pessoa e todas as suas transacoes em cascata.
    /// REGRA DE NEGOCIO: todas as transacoes associadas a pessoa sao removidas automaticamente.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa a ser deletada</param>
    /// <returns>Confirmacao de delecao (sem conteudo)</returns>
    /// <response code="204">Sucesso, pessoa e transacoes deletadas</response>
    /// <response code="404">Pessoa nao encontrada</response>
    [HttpDelete("{pessoaId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePessoa([FromRoute] Guid pessoaId)
    {
        await pessoaService.DeletePessoaAsync(pessoaId);
        return NoContent();
    }

    /// <summary>
    /// Retorna relatorio de totais financeiros agrupados por pessoa.
    /// Para cada pessoa, exibe: total de receitas, total de despesas e saldo (receita - despesa).
    /// Ao final, exibe os totais gerais somados de todas as pessoas.
    /// </summary>
    /// <returns>Relatorio com totais por pessoa e totais gerais</returns>
    /// <response code="200">Sucesso, retorna o relatorio de totais</response>
    [HttpGet("totais")]
    [ProducesResponseType(typeof(RelatorioTotaisResponse<TotalPorPessoaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RelatorioTotaisResponse<TotalPorPessoaResponse>>> GetTotaisPorPessoa()
    {
        var relatorio = await pessoaService.GetTotaisPorPessoaAsync();
        var response = relatorio.Adapt<RelatorioTotaisResponse<TotalPorPessoaResponse>>();
        return Ok(response);
    }
}
