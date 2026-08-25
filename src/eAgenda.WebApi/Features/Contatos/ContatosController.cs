using eAgenda.Aplicacao.Modulos.ModuloContato;
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
        {
            if (resultadoCadastro.HasError(e =>
                e.Message.Equals("Já existe um contato com este email.") ||
                e.Message.Equals("Já existe um contato com este telefone.")
            )
            )
            {
                return Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    detail: resultadoCadastro.Errors.First().Message,
                    title: "Conflito",
                    type: "https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Reference/Status/409"
                );
            }

            // Erros de Validação
            var modelState = new ModelStateDictionary();

            foreach (var erro in resultadoCadastro.Errors)
            {
                var campo = erro.Metadata["Campo"];

                modelState.AddModelError(campo.ToString()!, erro.Message);
            }

            ValidationProblemDetails problemDetails = new(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisição Inválida"
            };

            return ValidationProblem(problemDetails);
        }

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

        var resultado = servicoContato.Editar(dto);

        if (resultado.IsFailed)
            return NotFound(id);

        return NoContent();
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
