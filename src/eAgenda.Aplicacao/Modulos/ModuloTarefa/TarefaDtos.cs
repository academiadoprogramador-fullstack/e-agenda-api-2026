using eAgenda.Dominio.Modulos.ModuloTarefa;

namespace eAgenda.Aplicacao.Modulos.ModuloTarefa;

public record ListarTarefasDto(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade,
    DateTime DataCriacao,
    DateTime? DataConclusao,
    bool Concluida,
    int PercentualConcluido,
    List<ItemTarefaDto> Itens
);

public record CadastrarTarefaDto(
    string Titulo,
    PrioridadeTarefa Prioridade
);

public record EditarTarefaDto(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade
);

public record DetalhesTarefaDto(
    Guid Id,
    string Titulo,
    PrioridadeTarefa Prioridade,
    DateTime DataCriacao,
    DateTime? DataConclusao,
    bool Concluida,
    int PercentualConcluido,
    List<ItemTarefaDto> Itens
);

public record AdicionarItemTarefaDto(
    Guid TarefaId,
    string Titulo
);

public record AlterarConclusaoItemTarefaDto(
    Guid TarefaId,
    Guid ItemId,
    bool Concluido
);

public record RemoverItemTarefaDto(
    Guid TarefaId,
    Guid ItemId
);

public record AlterarConclusaoTarefaDto(
    Guid TarefaId,
    bool Concluida
);

public record ItemTarefaDto(
    Guid Id,
    string Titulo,
    bool Concluido
);
