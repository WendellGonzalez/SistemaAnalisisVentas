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
    public class CsvProductTransform : IProductSource
    {
        private readonly IExtractor<CsvProduct> _csvProducts;
        private readonly ILogger<CsvProductTransform> _logger;

        public CsvProductTransform(IExtractor<CsvProduct> csvProducts, ILogger<CsvProductTransform> logger)
        {
            _csvProducts = csvProducts;
            _logger = logger;
        }

        public async Task<List<Product>> GetCleanProductsAsync()
        {
            List<Product> products = new List<Product>();
            try
            {
                var productsCsv = await _csvProducts.ExtractAsync();

                products.AddRange(productsCsv.Select(p => new Product()
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price,
                    DataSource = "CSV"
                }));

                return products;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer products csv limpio");
                throw;
            }
        }
    }
}