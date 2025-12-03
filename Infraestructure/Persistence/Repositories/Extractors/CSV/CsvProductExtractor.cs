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
    public class CsvProductExtractor : IExtractor<CsvProduct>
    {
        private readonly string ruta;
        private readonly ILogger<CsvProductExtractor> _logger;
        private readonly IConfiguration _configuracion;

        public CsvProductExtractor(IConfiguration configuracion, ILogger<CsvProductExtractor> logger)
        {
            _logger = logger;
            ruta = configuracion["DataSources:Csv:Products"] ?? string.Empty;
            _configuracion = configuracion;
        }

        public async Task<IEnumerable<CsvProduct>> ExtractAsync()
        {
            List<CsvProduct> csvProducts = new List<CsvProduct>();
            try
            {
                _logger.LogInformation("Iniciando proceso de extraccion 'Products'. Ruta {ruta}", ruta);
                using var reader = new StreamReader(ruta);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var products = await csv.GetRecordsAsync<CsvProduct>().ToListAsync();

                csvProducts = products.Select(p => new CsvProduct
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    Stock = p.Stock
                }).ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al extraer los Productos del Csv. Ruta {ruta}", ruta);
                throw;
            }
            return csvProducts;
        }
    }
}