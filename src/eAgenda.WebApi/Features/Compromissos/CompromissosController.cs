using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.WebApi.Compartilhado;
using eAgenda.WebApi.Features.Compromissos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/compromissos")]
[Authorize]
public sealed class CompromissosController(ServicoCompromisso servico) : ControllerBase
{
    // Ação / Rota / Endpoint
    [HttpGet]
    public ActionResult<List<ListarCompromissosDto>> SelecionarTodos()
    {
        return Ok(servico.SelecionarTodos());
    }

    [HttpGet("{id:guid}")]
    public ActionResult<DetalhesCompromissoDto> SelecionarPorId(Guid id)
    {
        var resultadoSelecao = servico.SelecionarPorId(id);

        if (resultadoSelecao.IsFailed)
            return this.ProblemDetails(resultadoSelecao);

        return Ok(resultadoSelecao.Value);
    }

    [HttpPost]
    [ProducesResponseType<DetalhesCompromissoDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<DetalhesCompromissoDto> Cadastrar(CadastrarCompromissoRequest request)
    {
        var dto = new CadastrarCompromissoDto(
            request.Assunto,
            request.DataOcorrencia,
            request.HoraInicio,
            request.HoraTermino,
            request.Tipo,
            request.Local,
            request.Link,
            request.ContatoId
        );

        var resultadoCadastro = servico.Cadastrar(dto);

        if (resultadoCadastro.IsFailed)
            return this.ProblemDetails(resultadoCadastro);

        Guid id = resultadoCadastro.Value;

        var resultadoSelecao = servico.SelecionarPorId(id);

        if (resultadoSelecao.IsFailed)
            return this.ProblemDetails(resultadoSelecao);

        return CreatedAtAction(
            nameof(SelecionarPorId),
            new { id },
            resultadoSelecao.Value
        );
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult Editar(Guid id, EditarCompromissoRequest request)
    {
        var dto = new EditarCompromissoDto(
            id,
            request.Assunto,
            request.DataOcorrencia,
            request.HoraInicio,
            request.HoraTermino,
            request.Tipo,
            request.Local,
            request.Link,
            request.ContatoId
        );

        var resultadoEdicao = servico.Editar(dto);

        if (resultadoEdicao.IsFailed)
            return this.ProblemDetails(resultadoEdicao);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult Excluir(Guid id)
    {
        var resultadoExclusao = servico.Excluir(id);

        if (resultadoExclusao.IsFailed)
            return this.ProblemDetails(resultadoExclusao);

        return NoContent();
    }
}
