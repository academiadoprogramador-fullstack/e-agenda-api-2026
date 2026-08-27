using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using eAgenda.Aplicacao;
using eAgenda.Dominio.Compartilhado.Identity;
using eAgenda.Infra;
using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.WebApi.Compartilhado;
using eAgenda.WebApi.Compartilhado.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraRepositories(builder.Configuration, builder.Logging);
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProvedorDeUsuario, UserProvider>();

builder.Services.AddSingleton(provider =>
{
    var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>() ?? new JwtOptions();

    return new JwtProvider(jwtOptions);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName)
        .Get<JwtOptions>() ?? new JwtOptions();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtOptions.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtOptions.Audience,

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)), // Chave Mestra

        NameClaimType = ClaimTypes.NameIdentifier,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.ClientErrorMapping[StatusCodes.Status400BadRequest].Link = ProblemDetailsTypes.BadRequest;
        options.ClientErrorMapping[StatusCodes.Status401Unauthorized].Link = ProblemDetailsTypes.Unauthorized;
        options.ClientErrorMapping[StatusCodes.Status403Forbidden].Link = ProblemDetailsTypes.Forbidden;
        options.ClientErrorMapping[StatusCodes.Status404NotFound].Link = ProblemDetailsTypes.NotFound;
        options.ClientErrorMapping[StatusCodes.Status409Conflict].Link = ProblemDetailsTypes.Conflict;
    });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        string? type = ProblemDetailsTypes.ObterPorStatus(context.ProblemDetails.Status);

        if (type is not null)
            context.ProblemDetails.Type = type;

        if (context.ProblemDetails.Status == StatusCodes.Status401Unauthorized)
        {
            context.ProblemDetails.Title = "Não Autenticado";
            context.ProblemDetails.Detail = "É necessário fornecer credenciais válidas.";
        }
        else if (context.ProblemDetails.Status == StatusCodes.Status403Forbidden)
        {
            context.ProblemDetails.Title = "Acesso Negado";
            context.ProblemDetails.Detail = "O usuário autenticado não tem permissão para acessar este recurso.";
        }

        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    };
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT no formato: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var dbContext = scope.ServiceProvider.GetRequiredService<EAgendaDbContext>();

    dbContext.Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapControllers();

app.Run();
