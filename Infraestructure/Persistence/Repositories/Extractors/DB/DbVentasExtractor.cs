using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using CsvHelper;
using Domain.Entities.CSV;
using Domain.Entities.DB;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Persistence.Repositories.Extractors.DB
{
    public class DbVentasExtractor : IExtractor<VentaDB>
    {
        private readonly string connectionString;
        private readonly ILogger<DbVentasExtractor> logger;
        private readonly ContextDB context;
        private readonly IConfiguration _configuration;
        public DbVentasExtractor(IConfiguration configuration, ILogger<DbVentasExtractor> _logger, ContextDB _context)
        {
            connectionString = configuration["DataSources:Database:OperationalDB"] ?? string.Empty;
            logger = _logger;
            context = _context;
            _configuration = configuration;
        }
        public async Task<IEnumerable<VentaDB>> ExtractAsync()
        {
            List<VentaDB> ventaDBs = new List<VentaDB>();
            try
            {
                ventaDBs = await context.HistoricoVentas.ToListAsync();
                return ventaDBs;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error al extraer el historico de ventas de la DB");
            }
            return ventaDBs;
        }
    }
}