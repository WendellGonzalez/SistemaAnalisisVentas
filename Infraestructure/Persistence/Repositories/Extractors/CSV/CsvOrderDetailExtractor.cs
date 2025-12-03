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
    public class CsvOrderDetailExtractor : IExtractor<CsvOrderDetail>
    {
        private readonly string ruta;
        private readonly ILogger<CsvOrderDetailExtractor> _logger;
        private readonly IConfiguration _configuracion;

        public CsvOrderDetailExtractor(IConfiguration configuration, ILogger<CsvOrderDetailExtractor> logger)
        {
            ruta = configuration["DataSources:Csv:OrderDetails"]  ?? string.Empty;
            _logger = logger;
            _configuracion = configuration;
        }

        public async Task<IEnumerable<CsvOrderDetail>> ExtractAsync()
        {
            List<CsvOrderDetail> csvOrderDetails = new List<CsvOrderDetail>();
            try
            {
                _logger.LogInformation("Iniciando proceso de extraccion 'OrderDetails'. Ruta {ruta}", ruta);
                using var reader = new StreamReader(ruta);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var details = await csv.GetRecordsAsync<CsvOrderDetail>().ToListAsync();

                csvOrderDetails = details.Select(d => new CsvOrderDetail
                {
                    OrderID = d.OrderID,
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    TotalPrice = d.TotalPrice
                }).ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al extraer los OrderDetails del Csv. Ruta {ruta}", ruta);
            }
            return csvOrderDetails!;
        }
    }
}
