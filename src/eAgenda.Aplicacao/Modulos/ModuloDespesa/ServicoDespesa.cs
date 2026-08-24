using FluentResults;
using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using eAgenda.Aplicacao.Compartilhado;

namespace eAgenda.Aplicacao.Modulos.ModuloDespesa;

public class ServicoDespesa : ServicoBase<Despesa>
{
    private readonly IRepositorioDespesa repositorioDespesa;
    private readonly IRepositorioCategoria repositorioCategoria;

    public ServicoDespesa(
        IRepositorioDespesa repositorioDespesa,
        IRepositorioCategoria repositorioCategoria
    )
    {
        this.repositorioDespesa = repositorioDespesa;
        this.repositorioCategoria = repositorioCategoria;
    }

    public Result<Guid> Cadastrar(CadastrarDespesaDto dto)
    {
        Result<List<Categoria>> resultadoCategorias = SelecionarCategorias(dto.CategoriaIds);

        if (resultadoCategorias.IsFailed)
            return Result.Fail<Guid>(resultadoCategorias.Errors);

        Despesa novaDespesa = new Despesa(
            dto.Descricao,
            dto.DataOcorrencia?.Date ?? DateTime.Today,
            dto.Valor,
            dto.FormaPagamento,
            resultadoCategorias.Value
        );

        Result resultadoValidacao = ValidarEntidade(novaDespesa);

        if (resultadoValidacao.IsFailed)
            return Result.Fail<Guid>(resultadoValidacao.Errors);

        repositorioDespesa.Cadastrar(novaDespesa);

        return Result.Ok(novaDespesa.Id);
    }

    public Result Editar(EditarDespesaDto dto)
    {
        Result<List<Categoria>> resultadoCategorias = SelecionarCategorias(dto.CategoriaIds);

        if (resultadoCategorias.IsFailed)
            return resultadoCategorias.ToResult();

        Despesa despesaAtualizada = new Despesa(
            dto.Descricao,
            dto.DataOcorrencia?.Date ?? DateTime.Today,
            dto.Valor,
            dto.FormaPagamento,
            resultadoCategorias.Value
        );

        Result resultadoValidacao = ValidarEntidade(despesaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioDespesa.Editar(dto.Id, despesaAtualizada);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Despesa não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Despesa? despesa = repositorioDespesa.SelecionarPorId(id);

        if (despesa == null)
            return Falha(string.Empty, "Despesa não encontrada.");

        repositorioDespesa.Excluir(id);

        return Result.Ok();
    }

    public List<ListarDespesasDto> SelecionarTodos()
    {
        return Mapear(repositorioDespesa.SelecionarTodos());
    }

    public List<ListarDespesasDto> SelecionarTodos(Guid? categoriaId)
    {
        if (!categoriaId.HasValue || categoriaId == Guid.Empty)
            return SelecionarTodos();

        return Mapear(repositorioDespesa.Filtrar(d => d.Categorias.Any(c => c.Id == categoriaId.Value)));
    }

    private static List<ListarDespesasDto> Mapear(IEnumerable<Despesa> despesas)
    {
        return despesas
            .Select(d => new ListarDespesasDto(
                d.Id,
                d.Descricao,
                d.DataOcorrencia,
                d.Valor,
                d.FormaPagamento,
                d.Categorias.Select(c => new CategoriaDespesaDto(c.Id, c.Titulo)).ToList()
            ))
            .ToList();
    }

    public Result<DetalhesDespesaDto> SelecionarPorId(Guid id)
    {
        Despesa? despesa = repositorioDespesa.SelecionarPorId(id);

        if (despesa == null)
            return Result.Fail("Despesa não encontrada.");

        return Result.Ok(new DetalhesDespesaDto(
            despesa.Id,
            despesa.Descricao,
            despesa.DataOcorrencia,
            despesa.Valor,
            despesa.FormaPagamento,
            despesa.Categorias.Select(c => new CategoriaDespesaDto(c.Id, c.Titulo)).ToList()
        ));
    }

    public List<CategoriaDespesaDto> SelecionarCategorias()
    {
        return repositorioCategoria
            .SelecionarTodos()
            .Select(c => new CategoriaDespesaDto(c.Id, c.Titulo))
            .ToList();
    }

    private Result<List<Categoria>> SelecionarCategorias(List<Guid>? categoriaIds)
    {
        List<Guid> idsDistintos = (categoriaIds ?? [])
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList();

        if (idsDistintos.Count == 0)
            return Result.Fail<List<Categoria>>(new Error("Selecione ao menos uma categoria.").WithMetadata("Campo", nameof(CadastrarDespesaDto.CategoriaIds)));

        List<Categoria> categoriasSelecionadas = repositorioCategoria
            .SelecionarTodos()
            .Where(c => idsDistintos.Contains(c.Id))
            .ToList();

        if (categoriasSelecionadas.Count != idsDistintos.Count)
            return Result.Fail<List<Categoria>>(new Error("Selecione apenas categorias válidas.").WithMetadata("Campo", nameof(CadastrarDespesaDto.CategoriaIds)));

        return Result.Ok(categoriasSelecionadas);
    }
}
