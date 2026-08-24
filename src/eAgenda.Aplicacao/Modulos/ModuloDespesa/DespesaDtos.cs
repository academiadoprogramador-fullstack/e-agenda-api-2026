using eAgenda.Dominio.Modulos.ModuloDespesa;

namespace eAgenda.Aplicacao.Modulos.ModuloDespesa;

public record ListarDespesasDto(
    Guid Id,
    string Descricao,
    DateTime DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<CategoriaDespesaDto> Categorias
);

public record CadastrarDespesaDto(
    string Descricao,
    DateTime? DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<Guid> CategoriaIds
);

public record EditarDespesaDto(
    Guid Id,
    string Descricao,
    DateTime? DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<Guid> CategoriaIds
);

public record DetalhesDespesaDto(
    Guid Id,
    string Descricao,
    DateTime DataOcorrencia,
    decimal Valor,
    FormaPagamento FormaPagamento,
    List<CategoriaDespesaDto> Categorias
);

public record CategoriaDespesaDto(Guid Id, string Titulo);
