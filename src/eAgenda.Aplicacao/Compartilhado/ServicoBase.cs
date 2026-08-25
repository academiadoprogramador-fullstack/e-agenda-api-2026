using eAgenda.Dominio.Compartilhado;
using FluentResults;

namespace eAgenda.Aplicacao.Compartilhado;

public enum TipoErro
{
    Validacao,
    NaoEncontrado,
    Conflito
}

public abstract class ServicoBase<T> where T : EntidadeBase<T>
{
    protected static Result ValidarEntidade<TEntidade>(EntidadeBase<TEntidade> entidade)
    {
        IReadOnlyList<ErroValidacao> erros = entidade.Validar();

        if (erros.Count == 0)
            return Result.Ok();

        Result resultado = Result.Ok();

        foreach (ErroValidacao erro in erros)
            resultado.WithError(CriarErro(TipoErro.Validacao, erro.Campo, erro.Mensagem));

        return resultado;
    }

    protected static Result Falha(TipoErro tipo, string campo, string mensagem)
    {
        return Result.Fail(CriarErro(tipo, campo, mensagem));
    }

    protected static Result<TValue> Falha<TValue>(TipoErro tipo, string campo, string mensagem)
    {
        return Result.Fail<TValue>(CriarErro(tipo, campo, mensagem));
    }

    private static Error CriarErro(TipoErro tipo, string campo, string mensagem)
    {
        return new Error(mensagem)
            .WithMetadata(nameof(TipoErro), tipo)
            .WithMetadata("Campo", campo);
    }
}
