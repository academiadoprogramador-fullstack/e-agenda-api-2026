using eAgenda.Dominio.Modulos.ModuloCompromisso;

namespace eAgenda.WebApi.Features.Compromissos;

public record CadastrarCompromissoRequest(
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);

public record EditarCompromissoRequest(
    string Assunto,
    DateTime DataOcorrencia,
    TimeSpan HoraInicio,
    TimeSpan HoraTermino,
    TipoCompromisso Tipo,
    string? Local,
    string? Link,
    Guid? ContatoId
);
