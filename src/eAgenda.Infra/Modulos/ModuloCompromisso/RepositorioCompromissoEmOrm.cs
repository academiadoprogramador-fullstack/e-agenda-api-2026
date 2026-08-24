using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infra.Modulos.ModuloCompromisso;

public sealed class RepositorioCompromissoEmOrm(EAgendaDbContext dbContext) :
    RepositorioBaseEmOrm<Compromisso>(dbContext), IRepositorioCompromisso
{
    public override Compromisso? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(c => c.Contato)
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public override List<Compromisso> SelecionarTodos()
    {
        return registros
            .Include(c => c.Contato)
            .OrderBy(c => c.DataOcorrencia)
            .ThenBy(c => c.HoraInicio)
            .ToList();
    }

    public override List<Compromisso> Filtrar(Func<Compromisso, bool> filtro)
    {
        return registros
            .Include(c => c.Contato)
            .OrderBy(c => c.DataOcorrencia)
            .ThenBy(c => c.HoraInicio)
            .Where(filtro)
            .ToList();
    }
}
