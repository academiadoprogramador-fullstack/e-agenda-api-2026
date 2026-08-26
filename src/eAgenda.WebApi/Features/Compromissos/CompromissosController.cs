using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.WebApi.Compartilhado;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/compromissos")]
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

}
