using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Dominio.Modulos.ModuloContato;

namespace eAgenda.Infra.Modulos.ModuloContato;

public sealed class RepositorioContatoEmOrm(EAgendaDbContext dbContext) :
    RepositorioBaseEmOrm<Contato>(dbContext), IRepositorioContato
{
    public override List<Contato> SelecionarTodos()
    {
        return registros.OrderBy(c => c.Nome).ToList();
    }

    public override List<Contato> Filtrar(Func<Contato, bool> filtro)
    {
        return registros.Where(filtro).OrderBy(c => c.Nome).ToList();
    }
}
