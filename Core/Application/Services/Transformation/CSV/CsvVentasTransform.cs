using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.CSV;
using Microsoft.Extensions.Logging;


namespace Application.Services.Transformation.CSV
{
    public class CsvVentasTransform : IVentaSource
    {
        private readonly IExtractor<CsvOrder> _csvOrders;
        private readonly IExtractor<CsvOrderDetail> _csvOrderDetails;
        private readonly ILogger<CsvCustomerTransform> _logger;
        private readonly IProductSource _products;

        public CsvVentasTransform(IExtractor<CsvOrder> csvOrders, IExtractor<CsvOrderDetail> csvOrderDetails, IProductSource products, ILogger<CsvCustomerTransform> logger)
        {
            _csvOrderDetails = csvOrderDetails;
            _csvOrders = csvOrders;
            _products = products;
            _logger = logger;
        }
        public async Task<List<Venta>> GetCleanVentasAsync()
        {
            List<Venta> ventas = new List<Venta>();
            try
            {
                var ordersCsv = await _csvOrders.ExtractAsync();
                var detailsCsv = await _csvOrderDetails.ExtractAsync();
                var products = await _products.GetCleanProductsAsync();

                if (!ordersCsv.Any() || !detailsCsv.Any())
                {
                    return new List<Venta>();
                }

                ventas = (from order in ordersCsv
                          join detail in detailsCsv on order.OrderID equals detail.OrderID
                          join prod in products on detail.ProductID equals prod.ProductID
                          where detail.Quantity > 0
                          select new Venta()
                          {
                              OrderID = order.OrderID,
                              ProductID = detail.ProductID,
                              CustomerID = order.CustomerID,
                              OrderDate = order.OrderDate,
                              Quantity = detail.Quantity,
                              TotalPrice = detail.Quantity * prod.Price
                          }).ToList();

                return ventas;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer ventas del csv limpio");
                throw;
            }
        }
    }
}