namespace eAgenda.WebApi.Features.Contatos;

public record CadastrarContatoRequest(
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);

public record EditarContatoRequest(
    string Nome,
    string Email,
    string Telefone,
    string? Cargo,
    string? Empresa
);

