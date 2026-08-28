using eAgenda.Aplicacao.Modulos.ModuloContato;
using eAgenda.WebApi.Compartilhado.Http;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApi.Features.Contatos;

[ApiController]
[Route("api/contatos")]
public sealed class ContatosController(ServicoContato servicoContato) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<ListarContatosDto>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    public ActionResult<List<ListarContatosDto>> SelecionarTodos()
    {
        var resultadoSelecao = servicoContato.SelecionarTodos();

        return Ok(resultadoSelecao);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<DetalhesContatoDto>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public ActionResult<DetalhesContatoDto> SelecionarPorId(Guid id)
    {
        var resultadoSelecao = servicoContato.SelecionarPorId(id);

        if (resultadoSelecao.IsFailed)
            return this.ProblemDetails(resultadoSelecao);

        var dto = resultadoSelecao.Value;

        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType<DetalhesContatoDto>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public ActionResult<DetalhesContatoDto> Cadastrar(CadastrarContatoRequest req)
    {
        var dto = new CadastrarContatoDto(
            req.Nome,
            req.Email,
            req.Telefone,
            req.Cargo,
            req.Empresa
        );

        var resultadoCadastro = servicoContato.Cadastrar(dto);

        if (resultadoCadastro.IsFailed)
            return this.ProblemDetails(resultadoCadastro);

        var id = resultadoCadastro.Value;

        var resultadoSelecao = servicoContato.SelecionarPorId(id);

        if (resultadoSelecao.IsFailed)
            return NotFound(id);

        return CreatedAtAction(
            nameof(SelecionarPorId),
            new { id },
            resultadoSelecao.Value
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public ActionResult<DetalhesContatoDto> Editar(Guid id, EditarContatoRequest req)
    {
        var dto = new EditarContatoDto(
            id,
            req.Nome,
            req.Email,
            req.Telefone,
            req.Cargo,
            req.Empresa
        );

        var resultadoEdicao = servicoContato.Editar(dto);

        if (resultadoEdicao.IsFailed)
            return this.ProblemDetails(resultadoEdicao);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, "application/problem+json")]
    public ActionResult Excluir(Guid id)
    {
        var resultadoExclusao = servicoContato.Excluir(id);

        if (resultadoExclusao.IsFailed)
            return this.ProblemDetails(resultadoExclusao);

        return NoContent();
    }
}
