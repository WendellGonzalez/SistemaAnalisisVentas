using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities.API;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transformation.API
{
    public class ApiProductTransform : IProductSource
    {
        private readonly IExtractor<ApiProduct> _apiProducts;
        private readonly ILogger<ApiProductTransform> _logger;

        public ApiProductTransform(IExtractor<ApiProduct> apiProducts, ILogger<ApiProductTransform> logger)
        {
            _apiProducts = apiProducts;
            _logger = logger;
        }
        public async Task<List<Product>> GetCleanProductsAsync()
        {
            List<Product> products = new List<Product>();
            try
            {
                var productsApi = await _apiProducts.ExtractAsync();

                products.AddRange(productsApi.Select(p => new Product()
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.CategoryName,
                    Price = p.Price
                }));

                return products;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer productos de api limpios");
                throw;
            }
        }
    }
}