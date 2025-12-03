using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Collections;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services.Collections
{
    public class VentasCollection : IVentasCollection
    {
        private readonly IEnumerable<IVentaSource> _ventasSource;
        private readonly ILogger<VentasCollection> _logger;
        public VentasCollection(IEnumerable<IVentaSource> ventasSource, ILogger<VentasCollection> logger)
        {
            _ventasSource = ventasSource;
            _logger = logger;
        }
        public async Task<IEnumerable<Venta>> GetAllVentasAsync()
        {
            try
            {
                var allVentas = new List<Venta>();

                var tasks = _ventasSource.Select(source => source.GetCleanVentasAsync());

                var results = await Task.WhenAll(tasks);

                foreach (var v in results)
                {
                    allVentas.AddRange(v);
                }

                return allVentas
                .OrderBy(o => o.CustomerID)
                .GroupBy(v => v.OrderID)
                .Select(g => g.First())
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer ventas anidadas");
                throw;
            }
        }
    }
}