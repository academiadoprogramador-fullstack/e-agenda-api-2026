
using System.ComponentModel.DataAnnotations;

namespace eAgenda.WebApi.Features.Auth;

public sealed record RegistrarRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    [MinLength(8)]
    string Senha
);

public sealed record EntrarRequest(
    [Required]
    [EmailAddress]
    string Email,

    [Required]
    string Senha
);

public sealed record UsuarioResponse(Guid Id, string Email);
