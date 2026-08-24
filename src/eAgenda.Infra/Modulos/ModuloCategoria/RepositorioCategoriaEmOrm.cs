using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Dominio.Modulos.ModuloCategoria;

namespace eAgenda.Infra.Modulos.ModuloCategoria;

public sealed class RepositorioCategoriaEmOrm(EAgendaDbContext dbContext) :
    RepositorioBaseEmOrm<Categoria>(dbContext), IRepositorioCategoria
{
}
