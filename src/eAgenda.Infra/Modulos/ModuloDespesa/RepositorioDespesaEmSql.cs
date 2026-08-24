using Dapper;
using eAgenda.Infra.Compartilhado.Sql;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infra.Modulos.ModuloDespesa;

public sealed class RepositorioDespesaEmSql(ISqlConnectionFactory connectionFactory)
    : RepositorioBaseSql<Despesa>(connectionFactory), IRepositorioDespesa
{
    protected override string InserirSql => InserirDespesaSql;
    protected override string AtualizarSql => AtualizarDespesaSql;
    protected override string ExcluirSql => ExcluirDespesaSql;
    protected override string SelecionarPorIdSql => SelecionarPorIdDespesaSql;
    protected override string SelecionarTodosSql => SelecionarTodasDespesasSql;

    private const string InserirDespesaSql = """
        INSERT INTO dbo.TBDespesa (Id, Descricao, DataOcorrencia, Valor, FormaPagamento)
        VALUES (@Id, @Descricao, @DataOcorrencia, @Valor, @FormaPagamento);
    """;

    private const string InserirCategoriaDespesaSql = """
        INSERT INTO dbo.TBDespesaCategoria (DespesaId, CategoriaId)
        VALUES (@DespesaId, @CategoriaId);
    """;

    private const string AtualizarDespesaSql = """
        UPDATE dbo.TBDespesa
        SET
            Descricao = @Descricao,
            DataOcorrencia = @DataOcorrencia,
            Valor = @Valor,
            FormaPagamento = @FormaPagamento
        WHERE Id = @Id;
    """;

    private const string ExcluirCategoriasDespesaSql = """
        DELETE FROM dbo.TBDespesaCategoria
        WHERE DespesaId = @DespesaId;
    """;

    private const string ExcluirDespesaSql = """
        DELETE FROM dbo.TBDespesa
        WHERE Id = @Id;
    """;

    private const string SelecionarPorIdDespesaSql = """
        SELECT
            d.Id AS DespesaId,
            d.Descricao,
            d.DataOcorrencia,
            d.Valor,
            d.FormaPagamento,
            c.Id AS CategoriaId,
            c.Titulo AS CategoriaTitulo
        FROM dbo.TBDespesa AS d
        LEFT JOIN dbo.TBDespesaCategoria AS dc
            ON dc.DespesaId = d.Id
        LEFT JOIN dbo.TBCategoria AS c
            ON c.Id = dc.CategoriaId
        WHERE d.Id = @Id
        ORDER BY c.Titulo;
    """;

    private const string SelecionarTodasDespesasSql = """
        SELECT
            d.Id AS DespesaId,
            d.Descricao,
            d.DataOcorrencia,
            d.Valor,
            d.FormaPagamento,
            c.Id AS CategoriaId,
            c.Titulo AS CategoriaTitulo
        FROM dbo.TBDespesa AS d
        LEFT JOIN dbo.TBDespesaCategoria AS dc
            ON dc.DespesaId = d.Id
        LEFT JOIN dbo.TBCategoria AS c
            ON c.Id = dc.CategoriaId
        ORDER BY d.DataOcorrencia DESC, d.Descricao, c.Titulo;
    """;

    public override void Cadastrar(Despesa entidade)
    {
        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        conexao.Execute(InserirDespesaSql, CriarParametros(entidade), transacao);
        InserirCategorias(entidade, conexao, transacao);

        transacao.Commit();
    }

    public override bool Editar(Guid idSelecionado, Despesa entidadeAtualizada)
    {
        entidadeAtualizada.Id = idSelecionado;

        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        bool conseguiuEditar = conexao.Execute(
            AtualizarDespesaSql,
            CriarParametros(entidadeAtualizada),
            transacao
        ) == 1;

        if (!conseguiuEditar)
        {
            transacao.Rollback();

            return false;
        }

        conexao.Execute(ExcluirCategoriasDespesaSql, new { DespesaId = idSelecionado }, transacao);
        InserirCategorias(entidadeAtualizada, conexao, transacao);

        transacao.Commit();

        return true;
    }

    public override bool Excluir(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        using SqlTransaction transacao = conexao.BeginTransaction();

        conexao.Execute(ExcluirCategoriasDespesaSql, new { DespesaId = idSelecionado }, transacao);

        bool conseguiuExcluir = conexao.Execute(ExcluirDespesaSql, new { Id = idSelecionado }, transacao) == 1;

        transacao.Commit();

        return conseguiuExcluir;
    }

    public override Despesa? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        List<DespesaRow> linhas = conexao
            .Query<DespesaRow>(SelecionarPorIdDespesaSql, new { Id = idSelecionado })
            .ToList();

        return MapearDespesas(linhas).SingleOrDefault();
    }

    public override List<Despesa> SelecionarTodos()
    {
        using SqlConnection conexao = AbrirConexao();

        List<DespesaRow> linhas = conexao.Query<DespesaRow>(SelecionarTodasDespesasSql).ToList();

        return MapearDespesas(linhas);
    }

    protected override object CriarParametros(Despesa despesa)
    {
        return new
        {
            despesa.Id,
            despesa.Descricao,
            DataOcorrencia = despesa.DataOcorrencia.Date,
            despesa.Valor,
            FormaPagamento = (int)despesa.FormaPagamento
        };
    }

    private static void InserirCategorias(
        Despesa despesa,
        SqlConnection conexao,
        SqlTransaction transacao
    )
    {
        foreach (Categoria categoria in despesa.Categorias)
        {
            conexao.Execute(
                InserirCategoriaDespesaSql,
                new { DespesaId = despesa.Id, CategoriaId = categoria.Id },
                transacao
            );
        }
    }

    private static List<Despesa> MapearDespesas(List<DespesaRow> linhas)
    {
        Dictionary<Guid, Despesa> despesasPorId = [];

        foreach (DespesaRow linha in linhas)
        {
            if (!despesasPorId.TryGetValue(linha.DespesaId, out Despesa? despesa))
            {
                despesa = new Despesa
                {
                    Id = linha.DespesaId,
                    Descricao = linha.Descricao,
                    DataOcorrencia = linha.DataOcorrencia.Date,
                    Valor = linha.Valor,
                    FormaPagamento = linha.FormaPagamento,
                    Categorias = []
                };

                despesasPorId.Add(linha.DespesaId, despesa);
            }

            if (linha.CategoriaId.HasValue)
            {
                despesa.Categorias.Add(new Categoria
                {
                    Id = linha.CategoriaId.Value,
                    Titulo = linha.CategoriaTitulo ?? string.Empty
                });
            }
        }

        return despesasPorId.Values.ToList();
    }
}

public sealed class DespesaRow
{
    public Guid DespesaId { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public decimal Valor { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public Guid? CategoriaId { get; set; }
    public string? CategoriaTitulo { get; set; }
}
