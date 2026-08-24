using eAgenda.Aplicacao.Modulos.ModuloCategoria;
using eAgenda.Aplicacao.Modulos.ModuloCompromisso;
using eAgenda.Aplicacao.Modulos.ModuloContato;
using eAgenda.Aplicacao.Modulos.ModuloDespesa;
using eAgenda.Aplicacao.Modulos.ModuloTarefa;
using Microsoft.Extensions.DependencyInjection;

namespace eAgenda.Aplicacao;

public static class InjecaoDependencia
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ServicoContato>();
        services.AddScoped<ServicoCompromisso>();
        services.AddScoped<ServicoCategoria>();
        services.AddScoped<ServicoDespesa>();
        services.AddScoped<ServicoTarefa>();
    }
}
