using eAgenda.WebApi.Compartilhado.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eAgenda.WebApi.Features.Auth;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    JwtProvider jwtProvider
) : ControllerBase
{
    [HttpPost("registrar")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    public async Task<ActionResult<UsuarioResponse>> Registrar(RegistrarRequest request)
    {
        var usuario = new IdentityUser<Guid>()
        {
            Id = Guid.CreateVersion7(),
            UserName = request.Email.Trim(),
            Email = request.Email.Trim()
        };

        var resultado = await userManager.CreateAsync(usuario, request.Senha);

        if (!resultado.Succeeded)
        {
            foreach (IdentityError erro in resultado.Errors)
                ModelState.AddModelError(string.Empty, erro.Description);

            return ValidationProblem(ModelState);
        }

        return Created(string.Empty, new UsuarioResponse(usuario.Id, usuario.Email));
    }

    [HttpPost("entrar")]
    [ProducesResponseType<AccessTokenResponse>(StatusCodes.Status201Created, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, "application/problem+json")]
    public async Task<ActionResult<AccessTokenResponse>> Entrar(EntrarRequest request)
    {
        var usuario = await userManager.FindByEmailAsync(request.Email.Trim());

        if (usuario is null)
            return Unauthorized();

        var resultado = await signInManager.CheckPasswordSignInAsync(usuario, request.Senha, true);

        if (!resultado.Succeeded)
            return Unauthorized();

        var token = jwtProvider.CriarToken(usuario);

        return Ok(token);
    }
}
