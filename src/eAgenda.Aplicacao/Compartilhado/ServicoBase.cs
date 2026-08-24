using eAgenda.Dominio.Compartilhado;
using FluentResults;

namespace eAgenda.Aplicacao.Compartilhado;

public abstract class ServicoBase<T> where T : EntidadeBase<T>
{
    protected static Result ValidarEntidade(T entidade)
    {
        List<string> erros = entidade.Validar();

        if (erros.Count == 0)
            return Result.Ok();

        Result resultado = Result.Ok();

        foreach (string erro in erros)
            resultado.WithError(new Error(erro).WithMetadata("Campo", string.Empty));

        return resultado;
    }

    protected static Result Falha(string campo, string mensagem)
    {
        return Result.Fail(new Error(mensagem).WithMetadata("Campo", campo));
    }

    protected static Result<TValue> Falha<TValue>(string campo, string mensagem)
    {
        return Result.Fail<TValue>(new Error(mensagem).WithMetadata("Campo", campo));
    }
}
