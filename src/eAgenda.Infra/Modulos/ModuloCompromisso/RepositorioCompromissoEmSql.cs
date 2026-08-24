using Dapper;
using eAgenda.Infra.Compartilhado.Sql;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using Microsoft.Data.SqlClient;

namespace eAgenda.Infra.Modulos.ModuloCompromisso;

public class RepositorioCompromissoEmSql(ISqlConnectionFactory connectionFactory) :
    RepositorioBaseSql<Compromisso>(connectionFactory), IRepositorioCompromisso
{
    protected override string InserirSql => """
        INSERT INTO dbo.TBCompromisso
            (Id, Assunto, DataOcorrencia, HoraInicio, HoraTermino, Tipo, Local, Link, ContatoId)
        VALUES
            (@Id, @Assunto, @DataOcorrencia, @HoraInicio, @HoraTermino, @Tipo, @Local, @Link, @ContatoId);
    """;

    protected override string AtualizarSql => """
        UPDATE dbo.TBCompromisso
        SET
            Assunto = @Assunto,
            DataOcorrencia = @DataOcorrencia,
            HoraInicio = @HoraInicio,
            HoraTermino = @HoraTermino,
            Tipo = @Tipo,
            Local = @Local,
            Link = @Link,
            ContatoId = @ContatoId
        WHERE Id = @Id;
    """;

    protected override string ExcluirSql => """
        DELETE FROM dbo.TBCompromisso
        WHERE Id = @Id;
    """;

    protected override string SelecionarPorIdSql => """
        SELECT
            cp.Id AS CompromissoId,
            cp.Assunto,
            cp.DataOcorrencia,
            cp.HoraInicio,
            cp.HoraTermino,
            cp.Tipo,
            cp.Local,
            cp.Link,
            ct.Id AS ContatoId,
            ct.Nome AS ContatoNome,
            ct.Email AS ContatoEmail,
            ct.Telefone AS ContatoTelefone,
            ct.Cargo AS ContatoCargo,
            ct.Empresa AS ContatoEmpresa
        FROM dbo.TBCompromisso AS cp
        LEFT JOIN dbo.TBContato AS ct
            ON ct.Id = cp.ContatoId
        WHERE cp.Id = @Id;
    """;

    protected override string SelecionarTodosSql => """
        SELECT
            cp.Id AS CompromissoId,
            cp.Assunto,
            cp.DataOcorrencia,
            cp.HoraInicio,
            cp.HoraTermino,
            cp.Tipo,
            cp.Local,
            cp.Link,
            ct.Id AS ContatoId,
            ct.Nome AS ContatoNome,
            ct.Email AS ContatoEmail,
            ct.Telefone AS ContatoTelefone,
            ct.Cargo AS ContatoCargo,
            ct.Empresa AS ContatoEmpresa
        FROM dbo.TBCompromisso AS cp
        LEFT JOIN dbo.TBContato AS ct
            ON ct.Id = cp.ContatoId
        ORDER BY cp.DataOcorrencia, cp.HoraInicio;
    """;

    public override Compromisso? SelecionarPorId(Guid idSelecionado)
    {
        using SqlConnection conexao = AbrirConexao();

        CompromissoRow? compromissoRow = conexao.QuerySingleOrDefault<CompromissoRow>(
            SelecionarPorIdSql,
            new { Id = idSelecionado }
        );

        if (compromissoRow == null)
            return null;

        return MapearCompromisso(compromissoRow);
    }

    public override List<Compromisso> SelecionarTodos()
    {
        using SqlConnection conexao = AbrirConexao();

        return conexao
            .Query<CompromissoRow>(SelecionarTodosSql)
            .Select(MapearCompromisso)
            .ToList();
    }

    private static Compromisso MapearCompromisso(CompromissoRow row)
    {
        return new Compromisso
        {
            Id = row.CompromissoId,
            Assunto = row.Assunto,
            DataOcorrencia = row.DataOcorrencia.Date,
            HoraInicio = row.HoraInicio,
            HoraTermino = row.HoraTermino,
            Tipo = row.Tipo,
            Local = row.Local,
            Link = row.Link,
            Contato = row.ContatoId.HasValue
                ? new Contato
                {
                    Id = row.ContatoId.Value,
                    Nome = row.ContatoNome ?? string.Empty,
                    Email = row.ContatoEmail ?? string.Empty,
                    Telefone = row.ContatoTelefone ?? string.Empty,
                    Cargo = row.ContatoCargo,
                    Empresa = row.ContatoEmpresa
                }
                : null
        };
    }

    protected override object CriarParametros(Compromisso compromisso)
    {
        return new
        {
            compromisso.Id,
            compromisso.Assunto,
            DataOcorrencia = compromisso.DataOcorrencia.Date,
            compromisso.HoraInicio,
            compromisso.HoraTermino,
            Tipo = (int)compromisso.Tipo,
            compromisso.Local,
            compromisso.Link,
            ContatoId = compromisso.Contato?.Id
        };
    }
}

public sealed class CompromissoRow
{
    public Guid CompromissoId { get; set; }
    public string Assunto { get; set; } = string.Empty;
    public DateTime DataOcorrencia { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraTermino { get; set; }
    public TipoCompromisso Tipo { get; set; }
    public string? Local { get; set; }
    public string? Link { get; set; }
    public Guid? ContatoId { get; set; }
    public string? ContatoNome { get; set; }
    public string? ContatoEmail { get; set; }
    public string? ContatoTelefone { get; set; }
    public string? ContatoCargo { get; set; }
    public string? ContatoEmpresa { get; set; }
}
