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
    public class ProductsCollection : IProductsCollection
    {
        private readonly IEnumerable<IProductSource> _productsSource;
        private readonly ILogger<ProductsCollection> _logger;
        public ProductsCollection(IEnumerable<IProductSource> productsSource, ILogger<ProductsCollection> logger)
        {
            _productsSource = productsSource;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            try
            {
                var allProducts = new List<Product>();

                var tasks = _productsSource.Select(source => source.GetCleanProductsAsync());
                var results = await Task.WhenAll(tasks);

                foreach (var p in results)
                {
                    allProducts.AddRange(p);
                }

                return allProducts
                .GroupBy(p => p.ProductID)
                .Select(g => g.First())
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer productos anidados");
                throw;
            }
        }

    }
}