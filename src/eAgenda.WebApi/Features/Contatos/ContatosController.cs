using eAgenda.Aplicacao.Modulos.ModuloContato;
using eAgenda.WebApi.Compartilhado;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eAgenda.WebApi.Features.Contatos;

[ApiController]
[Route("api/contatos")]
public sealed class ContatosController(ServicoContato servicoContato) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<ListarContatosDto>> SelecionarTodos()
    {
        var resultadoSelecao = servicoContato.SelecionarTodos();

        return Ok(resultadoSelecao);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<DetalhesContatoDto> SelecionarPorId(Guid id)
    {
        var resultadoSelecao = servicoContato.SelecionarPorId(id);

        if (resultadoSelecao.IsFailed)
            return this.ParaErroDaApi(resultadoSelecao);

        var dto = resultadoSelecao.Value;

        return Ok(dto);
    }

    [HttpPost]
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
            return this.ParaErroDaApi(resultadoCadastro);

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
            return this.ParaErroDaApi(resultadoEdicao);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public ActionResult Excluir(Guid id)
    {
        var resultadoExclusao = servicoContato.Excluir(id);

        if (resultadoExclusao.IsFailed)
            return NotFound(id);

        return NoContent();
    }
}
