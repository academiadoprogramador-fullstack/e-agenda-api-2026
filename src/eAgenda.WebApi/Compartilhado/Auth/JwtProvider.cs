using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace eAgenda.WebApi.Compartilhado.Auth;

public sealed record AccessTokenResponse(string AccessToken, DateTime DataExpiracaoEmUtc);

public sealed class JwtProvider(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions options = jwtOptions.Value;

    public AccessTokenResponse CriarToken(IdentityUser<Guid> user)
    {
        DateTime dataCriacao = DateTime.UtcNow;
        DateTime dataExpiracao = dataCriacao.AddMinutes(options.AccessTokenMinutes);

        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
        ];

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(options.Key));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: dataCriacao,
            expires: dataExpiracao,
            signingCredentials: credentials
        );

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AccessTokenResponse(accessToken, dataExpiracao);
    }
}
