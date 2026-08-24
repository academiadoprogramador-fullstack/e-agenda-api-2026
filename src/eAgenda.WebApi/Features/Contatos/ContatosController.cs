using eAgenda.Aplicacao.Modulos.ModuloContato;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApi.Features.Contatos;

[ApiController]
[Route("api/contatos")]
public sealed class ContatosController(ServicoContato servicoContato) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ListarContatosDto>?> SelecionarTodos()
    {
        var resultado = servicoContato.SelecionarTodos();

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<DetalhesContatoDto> SelecionarPorId(Guid id)
    {
        var resultado = servicoContato.SelecionarPorId(id);

        if (resultado.IsFailed)
            return NotFound(id);

        var dto = resultado.Value;

        return Ok(dto);
    }

    [HttpPost]
    public ActionResult<CadastrarContatoResponse> Cadastrar(CadastrarContatoRequest req)
    {
        var dto = new CadastrarContatoDto(
            req.Nome,
            req.Email,
            req.Telefone,
            req.Cargo,
            req.Empresa
        );

        var resultado = servicoContato.Cadastrar(dto);

        if (resultado.IsFailed)
            return BadRequest();

        var res = new CadastrarContatoResponse(resultado.Value);

        return CreatedAtAction(nameof(SelecionarPorId), new { id = resultado.Value }, req);
    }

    [HttpPut("{id:guid}")]
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

        var resultado = servicoContato.Editar(dto);

        if (resultado.IsFailed)
            return NotFound(id);

        return CreatedAtAction(nameof(SelecionarPorId), new { id }, req);
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Excluir(Guid id)
    {
        var resultado = servicoContato.Excluir(id);

        if (resultado.IsFailed)
            return NotFound(id);

        return NoContent();
    }
}
