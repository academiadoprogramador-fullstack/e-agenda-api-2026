using Serilog;
using Serilog.Core;
using Serilog.Events;
using Microsoft.Extensions.Configuration;

namespace eAgenda.Infra.Compartilhado.Logging;

public static class SerilogFactory
{
    public static Logger Create(IConfiguration configuration)
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

        NewRelicOptions newRelicOptions = configuration
            .GetSection(NewRelicOptions.SectionName)
            .Get<NewRelicOptions>() ?? new NewRelicOptions();

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
