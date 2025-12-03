using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.DB;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transformation.DB
{
    public class DbVentasTransform : IVentaSource
    {
        private readonly IExtractor<VentaDB> _DbVentas;
        private readonly ILogger<DbVentasTransform> _logger;

        public DbVentasTransform(IExtractor<VentaDB> DbVentas, ILogger<DbVentasTransform> logger)
        {
            _DbVentas = DbVentas;
            _logger = logger;
        }
        public async Task<List<Venta>> GetCleanVentasAsync()
        {
            List<Venta> ventas;
            try
            {
                var ventasDB =  await _DbVentas.ExtractAsync();
                
                ventas = ventasDB.Select(v => new Venta
                {
                    OrderID = v.OrderID,
                    ProductID = v.ProductID,
                    CustomerID = v.CustomerID,
                    OrderDate = v.OrderDate,
                    Quantity = v.Quantity,
                    TotalPrice = v.TotalPrice
                }).ToList();

                return ventas.ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al cargar Ventas de la db limpios");
                throw;
            }
        }
    }
}