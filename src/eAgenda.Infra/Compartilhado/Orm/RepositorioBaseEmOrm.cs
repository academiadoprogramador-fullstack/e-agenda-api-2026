using eAgenda.Dominio.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace eAgenda.Infra.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrm<T>(EAgendaDbContext dbContext) where T : EntidadeBase<T>
{
    protected readonly DbSet<T> registros = dbContext.Set<T>();

    public void Cadastrar(T entidade)
    {
        registros.Add(entidade);

        dbContext.SaveChanges(); // commit
    }

    public bool Editar(Guid idSelecionado, T entidadeAtualizada)
    {
        T? registroSelecionado = SelecionarPorId(idSelecionado);

        if (registroSelecionado == null)
            return false;

        registroSelecionado.Atualizar(entidadeAtualizada);

        dbContext.SaveChanges();

        return true;
    }

    public bool Excluir(Guid idSelecionado)
    {
        T? TSelecionado = SelecionarPorId(idSelecionado);

        if (TSelecionado == null)
            return false;

        registros.Remove(TSelecionado);

        dbContext.SaveChanges();

        return true;
    }

    public virtual T? SelecionarPorId(Guid idSelecionado)
    {
        return registros.SingleOrDefault(c => c.Id == idSelecionado);
    }

    public virtual List<T> SelecionarTodos()
    {
        return registros.ToList();
    }

    public virtual List<T> Filtrar(Func<T, bool> filtro)
    {
        return registros.Where(filtro).ToList();
    }
}
