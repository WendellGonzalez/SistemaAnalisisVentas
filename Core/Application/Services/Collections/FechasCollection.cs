using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Collections;
using Microsoft.Extensions.Logging;

namespace Application.Services.Collections
{
    public class FechasCollection : IFechasCollection
    {
        private readonly IVentasCollection _ventasCollection;
        private readonly ILogger<FechasCollection> _logger;

        public FechasCollection(IVentasCollection ventasCollection, ILogger<FechasCollection> logger)
        {
            _ventasCollection = ventasCollection;
            _logger = logger;
        }

        public async Task<IEnumerable<DateTime>> GetAllFechasAsync()
        {
            try
            {
                var ventas = await _ventasCollection.GetAllVentasAsync();

                return ventas
                .Select(v => v.OrderDate.Date)
                .Distinct()
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error en Fechas collection al obtener coleccion de Fechas");
                throw;
            }
        }
    }
}