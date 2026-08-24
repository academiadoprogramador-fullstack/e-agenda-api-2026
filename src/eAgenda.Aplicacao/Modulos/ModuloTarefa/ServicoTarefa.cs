using FluentResults;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using eAgenda.Aplicacao.Compartilhado;

namespace eAgenda.Aplicacao.Modulos.ModuloTarefa;

public class ServicoTarefa : ServicoBase<Tarefa>
{
    private readonly IRepositorioTarefa repositorioTarefa;

    public ServicoTarefa(IRepositorioTarefa repositorioTarefa)
    {
        this.repositorioTarefa = repositorioTarefa;
    }

    public Result<Guid> Cadastrar(CadastrarTarefaDto dto)
    {
        Tarefa novaTarefa = new Tarefa(dto.Titulo, dto.Prioridade);

        Result resultadoValidacao = ValidarEntidade(novaTarefa);

        if (resultadoValidacao.IsFailed)
            return Result.Fail<Guid>(resultadoValidacao.Errors);

        repositorioTarefa.Cadastrar(novaTarefa);

        return Result.Ok(novaTarefa.Id);
    }

    public Result Editar(EditarTarefaDto dto)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(dto.Id);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        tarefa.Titulo = dto.Titulo;
        tarefa.Prioridade = dto.Prioridade;

        Result resultadoValidacao = ValidarEntidade(tarefa);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioTarefa.Editar(dto.Id, tarefa);

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(id);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        repositorioTarefa.Excluir(id);

        return Result.Ok();
    }

    public Result AdicionarItem(AdicionarItemTarefaDto dto)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(dto.TarefaId);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        ItemTarefa novoItem = new ItemTarefa(dto.Titulo);

        Result resultadoValidacaoItem = ValidarItem(novoItem);

        if (resultadoValidacaoItem.IsFailed)
            return resultadoValidacaoItem;

        tarefa.AdicionarItem(novoItem);

        Result resultadoValidacaoTarefa = ValidarEntidade(tarefa);

        if (resultadoValidacaoTarefa.IsFailed)
            return resultadoValidacaoTarefa;

        repositorioTarefa.Editar(tarefa.Id, tarefa);

        return Result.Ok();
    }

    public Result AlterarConclusaoItem(AlterarConclusaoItemTarefaDto dto)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(dto.TarefaId);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        bool itemEncontrado = tarefa.AlterarConclusaoItem(dto.ItemId, dto.Concluido);

        if (!itemEncontrado)
            return Falha(string.Empty, "Item de tarefa não encontrado.");

        repositorioTarefa.Editar(tarefa.Id, tarefa);

        return Result.Ok();
    }

    public Result AlterarConclusao(AlterarConclusaoTarefaDto dto)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(dto.TarefaId);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        bool conseguiuAlterar = tarefa.AlterarConclusaoManual(dto.Concluida);

        if (!conseguiuAlterar)
            return Falha(string.Empty, "A conclusão desta tarefa deve ser controlada pelos itens cadastrados.");

        repositorioTarefa.Editar(tarefa.Id, tarefa);

        return Result.Ok();
    }

    public Result RemoverItem(RemoverItemTarefaDto dto)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(dto.TarefaId);

        if (tarefa == null)
            return Falha(string.Empty, "Tarefa não encontrada.");

        bool itemEncontrado = tarefa.RemoverItem(dto.ItemId);

        if (!itemEncontrado)
            return Falha(string.Empty, "Item de tarefa não encontrado.");

        repositorioTarefa.Editar(tarefa.Id, tarefa);

        return Result.Ok();
    }

    public List<ListarTarefasDto> SelecionarTodos(string filtro = "Todas")
    {
        IEnumerable<Tarefa> tarefas = repositorioTarefa.SelecionarTodos();

        tarefas = filtro switch
        {
            "Pendentes" => tarefas.Where(t => !t.Concluida),
            "Concluidas" => tarefas.Where(t => t.Concluida),
            _ => tarefas
        };

        return tarefas
            .Select(MapearListarDto)
            .ToList();
    }

    public Result<DetalhesTarefaDto> SelecionarPorId(Guid id)
    {
        Tarefa? tarefa = repositorioTarefa.SelecionarPorId(id);

        if (tarefa == null)
            return Result.Fail("Tarefa não encontrada.");

        return Result.Ok(new DetalhesTarefaDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Prioridade,
            tarefa.DataCriacao,
            tarefa.DataConclusao,
            tarefa.Concluida,
            tarefa.PercentualConcluido,
            MapearItens(tarefa)
        ));
    }

    private static ListarTarefasDto MapearListarDto(Tarefa tarefa)
    {
        return new ListarTarefasDto(
            tarefa.Id,
            tarefa.Titulo,
            tarefa.Prioridade,
            tarefa.DataCriacao,
            tarefa.DataConclusao,
            tarefa.Concluida,
            tarefa.PercentualConcluido,
            MapearItens(tarefa)
        );
    }

    private static List<ItemTarefaDto> MapearItens(Tarefa tarefa)
    {
        return tarefa.Itens
            .Select(i => new ItemTarefaDto(i.Id, i.Titulo, i.Concluido))
            .ToList();
    }

    private static Result ValidarItem(ItemTarefa item)
    {
        List<string> erros = item.Validar();

        if (erros.Count == 0)
            return Result.Ok();

        return Falha(nameof(AdicionarItemTarefaDto.Titulo), erros.First());
    }
}
