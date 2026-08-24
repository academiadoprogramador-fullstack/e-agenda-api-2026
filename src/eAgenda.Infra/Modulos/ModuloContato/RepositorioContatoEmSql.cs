using eAgenda.Infra.Compartilhado.Sql;
using eAgenda.Dominio.Modulos.ModuloContato;

namespace eAgenda.Infra.Modulos.ModuloContato;

public class RepositorioContatoEmSql(ISqlConnectionFactory connectionFactory) :
    RepositorioBaseSql<Contato>(connectionFactory), IRepositorioContato
{
    protected override string InserirSql => """
        INSERT INTO dbo.TBContato (Id, Nome, Email, Telefone, Cargo, Empresa)
        VALUES (@Id, @Nome, @Email, @Telefone, @Cargo, @Empresa);
    """;

    protected override string AtualizarSql => """
        UPDATE dbo.TBContato
        SET
            Nome = @Nome,
            Email = @Email,
            Telefone = @Telefone,
            Cargo = @Cargo,
            Empresa = @Empresa
        WHERE Id = @Id;
    """;

    protected override string ExcluirSql => """
        DELETE FROM dbo.TBContato
        WHERE Id = @Id;
    """;

    protected override string SelecionarPorIdSql => """
        SELECT Id, Nome, Email, Telefone, Cargo, Empresa
        FROM dbo.TBContato
        WHERE Id = @Id;
    """;

    protected override string SelecionarTodosSql => """
        SELECT Id, Nome, Email, Telefone, Cargo, Empresa
        FROM dbo.TBContato
        ORDER BY Nome;
    """;
}
