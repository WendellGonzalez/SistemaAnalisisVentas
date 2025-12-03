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
    public class CsvCustomerExtractor : IExtractor<CsvCustomer>
    {
        private readonly string ruta;
        private readonly ILogger<CsvCustomerExtractor> _logger;
        private readonly IConfiguration _configuracion;
        public CsvCustomerExtractor(IConfiguration configuracion, ILogger<CsvCustomerExtractor> logger)
        {
            _logger = logger;
            _configuracion = configuracion;
            ruta = configuracion["DataSources:Csv:Customers"]  ?? string.Empty;
        }

        public async Task<IEnumerable<CsvCustomer>> ExtractAsync()
        {
            List<CsvCustomer> csvCustomers = new List<CsvCustomer>();
            try
            {
                _logger.LogInformation("Iniciando proceso de extraccion 'Customers'. Ruta {ruta}", ruta);
                using var reader = new StreamReader(ruta);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var customers = await csv.GetRecordsAsync<CsvCustomer>().ToListAsync();

                csvCustomers = customers.Select(c => new CsvCustomer
                {
                    CustomerID = c.CustomerID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    City = c.City,
                    Country = c.Country
                }).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al extraer Customers del Csv. Ruta {ruta}", ruta);
            }
            return csvCustomers!;
        }
    }
}