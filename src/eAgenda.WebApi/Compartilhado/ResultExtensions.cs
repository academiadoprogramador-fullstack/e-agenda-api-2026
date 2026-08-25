using eAgenda.Aplicacao.Compartilhado;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eAgenda.WebApi.Compartilhado;

public static class ResultExtensions
{
    public static ActionResult ObterValidationProblem(this ControllerBase controller, ResultBase result)
    {
        var tipoErro = (TipoErro)result.Errors.First().Metadata[nameof(TipoErro)];

        if (tipoErro == TipoErro.NaoEncontrado)
        {
            return controller.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: result.Errors.First().Message,
                title: "Recurso Não Encontrado",
                type: "https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Reference/Status/404"
            );
        }

        if (tipoErro == TipoErro.Conflito)
        {
            return controller.Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: result.Errors.First().Message,
                title: "Conflito",
                type: "https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Reference/Status/409"
            );
        }

        // Erros de Validação
        var modelState = new ModelStateDictionary();

        foreach (var erro in result.Errors)
        {
            var campo = erro.Metadata["Campo"].ToString()!;

            modelState.AddModelError(campo, erro.Message);
        }

        ValidationProblemDetails problemDetails = new(modelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Requisição Inválida"
        };

        return controller.ValidationProblem(problemDetails);
    }
}
