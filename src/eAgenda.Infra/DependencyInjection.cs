using eAgenda.Dominio.Modulos.ModuloCategoria;
using eAgenda.Dominio.Modulos.ModuloCompromisso;
using eAgenda.Dominio.Modulos.ModuloContato;
using eAgenda.Dominio.Modulos.ModuloDespesa;
using eAgenda.Dominio.Modulos.ModuloTarefa;
using eAgenda.Infra.Compartilhado.Orm;
using eAgenda.Infra.Modulos.ModuloCategoria;
using eAgenda.Infra.Modulos.ModuloCompromisso;
using eAgenda.Infra.Modulos.ModuloContato;
using eAgenda.Infra.Modulos.ModuloDespesa;
using eAgenda.Infra.Modulos.ModuloTarefa;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eAgenda.Infra;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDataProtection();

        services.AddIdentityCore<IdentityUser<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<EAgendaDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddDbContext<EAgendaDbContext>(options =>
       {
           string? connectionString = configuration.GetConnectionString("SqlServerEF");

           if (string.IsNullOrWhiteSpace(connectionString))
           {
               throw new InvalidOperationException(
                   $"A connection string \"SqlServerEF\" não foi encontrada."
               );
           }

           if (configuration["Infra:DatabaseProvider"] == "InMemory")
           {
               options.UseInMemoryDatabase("eAgenda");
           }
           else
           {
               options.UseSqlServer(connectionString, opt =>
               {
                   opt.EnableRetryOnFailure(3);
               });
           }
       });

        services.AddScoped<IRepositorioContato, RepositorioContatoEmOrm>();
        services.AddScoped<IRepositorioCompromisso, RepositorioCompromissoEmOrm>();
        services.AddScoped<IRepositorioCategoria, RepositorioCategoriaEmOrm>();
        services.AddScoped<IRepositorioDespesa, RepositorioDespesaEmOrm>();
        services.AddScoped<IRepositorioTarefa, RepositorioTarefaEmOrm>();
    }
}
