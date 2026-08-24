using Dapper;
using eAgenda.Infra.Compartilhado.Sql;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infra.Modulos.ModuloTarefa;

public sealed class RepositorioTarefaEmSql(ISqlConnectionFactory connectionFactory)
    : RepositorioBaseSql<Tarefa>(connectionFactory), IRepositorioTarefa
{
    protected override string InserirSql => InserirTarefaSql;
    protected override string AtualizarSql => AtualizarTarefaSql;
    protected override string ExcluirSql => ExcluirTarefaSql;
    protected override string SelecionarPorIdSql => SelecionarPorIdTarefaSql;
    protected override string SelecionarTodosSql => SelecionarTodasTarefasSql;

    private const string InserirTarefaSql = """
        INSERT INTO dbo.TBTarefa
            (Id, Titulo, Prioridade, DataCriacao, DataConclusao, Concluida, PercentualConcluido)
        VALUES
            (@Id, @Titulo, @Prioridade, @DataCriacao, @DataConclusao, @Concluida, @PercentualConcluido);
    """;

    private const string InserirItemSql = """
        INSERT INTO dbo.TBItemTarefa (Id, TarefaId, Titulo, Concluido)
        VALUES (@Id, @TarefaId, @Titulo, @Concluido);
    """;

    private const string AtualizarTarefaSql = """
        UPDATE dbo.TBTarefa
        SET
            Titulo = @Titulo,
            Prioridade = @Prioridade,
            DataCriacao = @DataCriacao,
            DataConclusao = @DataConclusao,
            Concluida = @Concluida,
            PercentualConcluido = @PercentualConcluido
        WHERE Id = @Id;
    """;

    private const string ExcluirItensSql = """
        DELETE FROM dbo.TBItemTarefa
        WHERE TarefaId = @TarefaId;
    """;

    private const string ExcluirTarefaSql = """
        DELETE FROM dbo.TBTarefa
        WHERE Id = @Id;
    """;

    private const string SelecionarPorIdTarefaSql = """
        SELECT
            t.Id AS TarefaId,
            t.Titulo AS TarefaTitulo,
            t.Prioridade,
            t.DataCriacao,
            t.DataConclusao,
            t.Concluida,
            t.PercentualConcluido,
            i.Id AS ItemId,
            i.Titulo AS ItemTitulo,
            i.Concluido AS ItemConcluido
        FROM dbo.TBTarefa AS t
        LEFT JOIN dbo.TBItemTarefa AS i
            ON i.TarefaId = t.Id
        WHERE t.Id = @Id
        ORDER BY i.Titulo;
    """;

    private const string SelecionarTodasTarefasSql = """
        SELECT
            t.Id AS TarefaId,
            t.Titulo AS TarefaTitulo,
            t.Prioridade,
            t.DataCriacao,
            t.DataConclusao,
            t.Concluida,
            t.PercentualConcluido,
            i.Id AS ItemId,
            i.Titulo AS ItemTitulo,
            i.Concluido AS ItemConcluido
        FROM dbo.TBTarefa AS t
        LEFT JOIN dbo.TBItemTarefa AS i
            ON i.TarefaId = t.Id
        ORDER BY t.Prioridade DESC, t.DataCriacao DESC, t.Titulo, i.Titulo;
    """;

    public override void Cadastrar(Tarefa entidade)
    {
        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        conexao.Execute(InserirTarefaSql, CriarParametrosTarefa(entidade), transacao);

        InserirItens(entidade, conexao, transacao);

        transacao.Commit();
    }

    public override bool Editar(Guid idSelecionado, Tarefa entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        bool conseguiuEditar = conexao.Execute(
            AtualizarTarefaSql,
            CriarParametrosTarefa(entidadeAtualizada),
            transacao
        ) == 1;

        if (!conseguiuEditar)
        {
            transacao.Rollback();

            return false;
        }

        conexao.Execute(ExcluirItensSql, new { TarefaId = idSelecionado }, transacao);
        InserirItens(entidadeAtualizada, conexao, transacao);

        transacao.Commit();

        return true;
    }

    public override bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        conexao.Execute(ExcluirItensSql, new { TarefaId = idSelecionado }, transacao);

        bool conseguiuExcluir = conexao.Execute(ExcluirTarefaSql, new { Id = idSelecionado }, transacao) == 1;

        transacao.Commit();

        return conseguiuExcluir;
    }

    public override Tarefa? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        List<TarefaRow> linhas = conexao
            .Query<TarefaRow>(SelecionarPorIdTarefaSql, new { Id = idSelecionado })
            .ToList();

        return MapearTarefas(linhas).SingleOrDefault();
    }

    public override List<Tarefa> SelecionarTodos()
    {
        using SqlConnection conexao = AbrirConexao();

        List<TarefaRow> linhas = conexao.Query<TarefaRow>(SelecionarTodasTarefasSql).ToList();

        return MapearTarefas(linhas);
    }

    private static object CriarParametrosTarefa(Tarefa tarefa)
    {
        return new
        {
            tarefa.Id,
            tarefa.Titulo,
            Prioridade = (int)tarefa.Prioridade,
            DataCriacao = tarefa.DataCriacao.Date,
            DataConclusao = tarefa.DataConclusao?.Date,
            tarefa.Concluida,
            tarefa.PercentualConcluido
        };
    }

    private static void InserirItens(
        Tarefa tarefa,
        SqlConnection conexao,
        SqlTransaction transacao
    )
    {
        foreach (ItemTarefa item in tarefa.Itens)
        {
            conexao.Execute(
                InserirItemSql,
                new
                {
                    item.Id,
                    TarefaId = tarefa.Id,
                    item.Titulo,
                    item.Concluido
                },
                transacao
            );
        }
    }

    private static List<Tarefa> MapearTarefas(List<TarefaRow> linhas)
    {
        Dictionary<Guid, Tarefa> tarefasPorId = [];

        foreach (TarefaRow linha in linhas)
        {
            if (!tarefasPorId.TryGetValue(linha.TarefaId, out Tarefa? tarefa))
            {
                tarefa = new Tarefa
                {
                    Id = linha.TarefaId,
                    Titulo = linha.TarefaTitulo,
                    Prioridade = linha.Prioridade,
                    DataCriacao = linha.DataCriacao.Date,
                    DataConclusao = linha.DataConclusao?.Date,
                    Concluida = linha.Concluida,
                    PercentualConcluido = linha.PercentualConcluido,
                    Itens = []
                };

                tarefasPorId.Add(linha.TarefaId, tarefa);
            }

            if (linha.ItemId.HasValue)
            {
                tarefa.Itens.Add(new ItemTarefa
                {
                    Id = linha.ItemId.Value,
                    Titulo = linha.ItemTitulo ?? string.Empty,
                    Concluido = linha.ItemConcluido
                });
            }
        }

        return tarefasPorId.Values.ToList();
    }
}

public sealed class TarefaRow
{
    public Guid TarefaId { get; set; }
    public string TarefaTitulo { get; set; } = string.Empty;
    public PrioridadeTarefa Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public bool Concluida { get; set; }
    public int PercentualConcluido { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemTitulo { get; set; }
    public bool ItemConcluido { get; set; }
}
