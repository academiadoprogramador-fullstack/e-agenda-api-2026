using FluentResults;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Aplicacao.Compartilhado;
using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Aplicacao.Modulos.ModuloCategoria;

public class ServicoCategoria : ServicoBase<Categoria>
{
    private readonly IRepositorioCategoria repositorioCategoria;
    private readonly IRepositorioDespesa repositorioDespesa;

    public ServicoCategoria(
        IRepositorioCategoria repositorioCategoria,
        IRepositorioDespesa repositorioDespesa
    )
    {
        this.repositorioCategoria = repositorioCategoria;
        this.repositorioDespesa = repositorioDespesa;
    }

    public Result<Guid> Cadastrar(CadastrarCategoriaDto dto)
    {
        if (ExisteCategoriaComMesmoTitulo(dto.Titulo))
            return Falha<Guid>(nameof(dto.Titulo), "Já existe uma categoria com este título.");

        Categoria novaCategoria = new Categoria(dto.Titulo);

        Result resultadoValidacao = ValidarEntidade(novaCategoria);

        if (resultadoValidacao.IsFailed)
            return Result.Fail<Guid>(resultadoValidacao.Errors);

        repositorioCategoria.Cadastrar(novaCategoria);

        return Result.Ok(novaCategoria.Id);
    }

    public Result Editar(EditarCategoriaDto dto)
    {
        if (ExisteCategoriaComMesmoTitulo(dto.Titulo, dto.Id))
            return Falha(nameof(dto.Titulo), "Já existe uma categoria com este título.");

        Categoria categoriaAtualizada = new Categoria(dto.Titulo);

        Result resultadoValidacao = ValidarEntidade(categoriaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioCategoria.Editar(dto.Id, categoriaAtualizada);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Categoria não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Categoria? categoria = repositorioCategoria.SelecionarPorId(id);

        if (categoria == null)
            return Falha(string.Empty, "Categoria não encontrada.");

        if (PossuiDespesasVinculadas(id))
            return Falha(string.Empty, "Não é possível excluir esta categoria, pois ela possui despesas vinculadas.");

        repositorioCategoria.Excluir(id);

        return Result.Ok();
    }

    public List<ListarCategoriasDto> SelecionarTodos()
    {
        return repositorioCategoria
            .SelecionarTodos()
            .Select(c => new ListarCategoriasDto(c.Id, c.Titulo))
            .ToList();
    }

    public Result<DetalhesCategoriaDto> SelecionarPorId(Guid id)
    {
        Categoria? categoria = repositorioCategoria.SelecionarPorId(id);

        if (categoria == null)
            return Result.Fail("Categoria não encontrada.");

        return Result.Ok(new DetalhesCategoriaDto(categoria.Id, categoria.Titulo));
    }

    private bool ExisteCategoriaComMesmoTitulo(string titulo, Guid? idIgnorado = null)
    {
        string tituloNormalizado = NormalizarTitulo(titulo);

        return repositorioCategoria
            .SelecionarTodos()
            .Any(c =>
                c.Id != idIgnorado &&
                NormalizarTitulo(c.Titulo) == tituloNormalizado
            );
    }

    private bool PossuiDespesasVinculadas(Guid categoriaId)
    {
        return repositorioDespesa
            .SelecionarTodos()
            .Any(d => d.Categorias.Any(c => c.Id == categoriaId));
    }

    private static string NormalizarTitulo(string titulo)
    {
        return titulo.Trim().ToLowerInvariant();
    }
}
