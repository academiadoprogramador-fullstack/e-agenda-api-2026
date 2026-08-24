using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eAgenda.Infra.Compartilhado.Logging;

public static class SerilogLoggerFactory
{
    public static void AddSerilogLogger(
        this IServiceCollection services,
        IConfiguration configuration,
        ILoggingBuilder logging
    )
    {
        Log.Logger = SerilogFactory.Create(configuration);

        // Remove o provedor padrão de logs da Microsoft
        logging.ClearProviders();

        services.AddSerilog(Log.Logger);
    }
}
