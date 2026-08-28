using Serilog;
using Serilog.Core;
using Serilog.Events;
using Microsoft.Extensions.Options;

namespace eAgenda.WebApi.Compartilhado.Logging;

public static class LoggingExtensions
{
    public static void AddSerilogServices(
        this IServiceCollection services,
        ILoggingBuilder logging
    )
    {
        using ServiceProvider sp = services.BuildServiceProvider();
        IOptions<NewRelicOptions> options = sp.GetRequiredService<IOptions<NewRelicOptions>>();

        Serilog.ILogger logger = CriarLogger(options.Value);

        // Remove o provedor padrão de logs da Microsoft e adiciona Serilog
        logging.ClearProviders();
        services.AddSerilog(logger);
    }

    public static Logger CriarLogger(NewRelicOptions newRelicOptions)
    {
        string caminhoAppData = Environment
            .GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string caminhoDiretorio = Path.Combine(caminhoAppData, "eAgenda");

        Directory.CreateDirectory(caminhoDiretorio);

        string caminhoLogs = Path.Combine(caminhoDiretorio, "erro.log");

        LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                caminhoLogs,
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Error
            );

        if (newRelicOptions.Enabled)
        {
            if (string.IsNullOrWhiteSpace(newRelicOptions.LicenseKey))
            {
                throw new InvalidOperationException(
                    "A chave de licença do New Relic não foi configurada. Configure Infra:NewRelic:LicenseKey."
                );
            }

            loggerConfiguration.WriteTo.NewRelicLogs(
                endpointUrl: newRelicOptions.EndpointUrl,
                applicationName: newRelicOptions.ApplicationName,
                licenseKey: newRelicOptions.LicenseKey
            );
        }

        return loggerConfiguration.CreateLogger();
    }
}
