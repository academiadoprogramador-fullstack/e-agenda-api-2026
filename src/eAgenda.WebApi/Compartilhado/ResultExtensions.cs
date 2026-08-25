using eAgenda.Aplicacao.Compartilhado;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eAgenda.WebApi.Compartilhado;

public static class ResultExtensions
{
    public static ActionResult ValidationProblem(this ControllerBase controller, ResultBase result)
    {
        var tipoErro = (TipoErro)result.Errors.First().Metadata[nameof(TipoErro)];

        if (tipoErro.Equals(TipoErro.NaoEncontrado))
        {
            return controller.Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: result.Errors.First().Message,
                title: "Recurso Não Encontrado",
                type: ProblemDetailsTypes.NotFound
            );
        }

        if (tipoErro.Equals(TipoErro.Conflito))
        {
            return controller.Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: result.Errors.First().Message,
                title: "Conflito",
                type: ProblemDetailsTypes.Conflict
            );
        }

        if (tipoErro.Equals(TipoErro.Validacao))
        {
            var modelState = new ModelStateDictionary();

            foreach (var erro in result.Errors)
            {
                var campo = erro.Metadata["Campo"].ToString()!;

                modelState.AddModelError(campo, erro.Message);
            }

            ValidationProblemDetails problemDetails = new(modelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Requisição Inválida",
                Type = ProblemDetailsTypes.BadRequest
            };

            return controller.ValidationProblem(problemDetails);
        }

        return controller.Problem(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Erro Interno do Servidor",
            type: ProblemDetailsTypes.InternalServerError
        );
    }
}
