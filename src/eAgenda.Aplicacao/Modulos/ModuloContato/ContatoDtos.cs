namespace eAgenda.Aplicacao.Modulos.ModuloContato;

public record ListarContatosDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);

public record CadastrarContatoDto(
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);

public record EditarContatoDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);

public record DetalhesContatoDto(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);
