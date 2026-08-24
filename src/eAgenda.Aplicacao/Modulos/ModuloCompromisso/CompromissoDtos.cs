using eAgenda.Dominio.Modulos.ModuloCompromisso;

namespace eAgenda.Aplicacao.Modulos.ModuloCompromisso;

public record ListarCompromissosDto(
    Guid Id,
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId,
    string? ContatoNome
);

public record CadastrarCompromissoDto(
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);

public record EditarCompromissoDto(
    Guid Id,
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);

public record DetalhesCompromissoDto(
    Guid Id,
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId,
    string? ContatoNome
);

public record OpcaoContatoDto(Guid Id, string Nome);
