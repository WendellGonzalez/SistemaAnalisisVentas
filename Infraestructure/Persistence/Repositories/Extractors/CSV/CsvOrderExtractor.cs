using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using CsvHelper;
using Domain.Entities.CSV;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Persistence.Repositories.Extractors.CSV
{
    public class CsvOrderExtractor : IExtractor<CsvOrder>
    {
        private readonly string ruta;
        private readonly ILogger<CsvOrderExtractor> _logger;
        private readonly IConfiguration _configuracion;

        public CsvOrderExtractor(IConfiguration configuration, ILogger<CsvOrderExtractor> logger)
        {
            ruta = configuration["DataSources:Csv:Orders"] ?? string.Empty;
            _logger = logger;
            _configuracion = configuration;
        }

        public async Task<IEnumerable<CsvOrder>> ExtractAsync()
        {
            List<CsvOrder> csvOrders = new List<CsvOrder>();
            try
            {

                _logger.LogInformation("Iniciando proceso de extraccion 'Orders'. Ruta {ruta}", ruta);
                using var reader = new StreamReader(ruta);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var orders = await csv.GetRecordsAsync<CsvOrder>().ToListAsync();

                csvOrders = orders.Select(o => new CsvOrder
                {
                    OrderID = o.OrderID,
                    CustomerID = o.CustomerID,
                    OrderDate = o.OrderDate,
                    Status = o.Status
                }).ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al extraer los Detalles de Ventas del Csv");
            }
            return csvOrders;
        }
    }
}

