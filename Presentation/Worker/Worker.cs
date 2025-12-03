using Application.Services;

namespace Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            try
            {
                _logger.LogInformation("Iniciando proceso de carga de datos a DWH...");

                await CollectionService.ExectuteETLProcess(_serviceProvider);

                // _logger.LogInformation("{Success} - {Message}", Result.Success, Result.Message);

                // Console.WriteLine("\n=========================================================================\n");
                // Console.WriteLine(Result.Success);
                // Console.WriteLine(Result.Message);
                // Console.WriteLine("\n=========================================================================\n");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error en la carga de datos a la Dwh");
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}
