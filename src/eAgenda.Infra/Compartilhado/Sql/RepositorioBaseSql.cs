using Dapper;
using eAgenda.Dominio.Compartilhado;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infra.Compartilhado.Sql;

public abstract class RepositorioBaseSql<T>(
    ISqlConnectionFactory connectionFactory
) where T : EntidadeBase<T>
{
    protected abstract string InserirSql { get; }
    protected abstract string AtualizarSql { get; }
    protected abstract string ExcluirSql { get; }
    protected abstract string SelecionarPorIdSql { get; }
    protected abstract string SelecionarTodosSql { get; }

    public virtual void Cadastrar(T entidade)
    {
        using SqlConnection conexao = AbrirConexao();

        conexao.Execute(InserirSql, CriarParametros(entidade));
    }

    public virtual bool Editar(Guid idSelecionado, T entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = AbrirConexao();

        return conexao.Execute(AtualizarSql, CriarParametros(entidadeAtualizada)) == 1;
    }

    public virtual bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        return conexao.Execute(ExcluirSql, new { Id = idSelecionado }) == 1;
    }

    public virtual T? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        return conexao.QuerySingleOrDefault<T>(SelecionarPorIdSql, new { Id = idSelecionado });
    }

    public virtual List<T> SelecionarTodos()
    {
        using SqlConnection conexao = AbrirConexao();

        return conexao.Query<T>(SelecionarTodosSql).ToList();
    }

    public List<T> Filtrar(Func<T, bool> filtro)
    {
        return SelecionarTodos().Where(filtro).ToList();
    }

    protected SqlConnection AbrirConexao()
    {
        SqlConnection conexao = connectionFactory.CreateConnection();

        conexao.Open();

        return conexao;
    }

    protected virtual object CriarParametros(T entidade)
    {
        return entidade;
    }
}
