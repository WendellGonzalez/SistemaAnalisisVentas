using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.API;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Persistence.Repositories.Extractors.API
{
    public class ApiProductExtractor : IExtractor<ApiProduct>
    {
        private readonly ILogger<ApiProductExtractor> _logger;
        private string baseUrl;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuracion;

        public ApiProductExtractor(IConfiguration configuration, ILogger<ApiProductExtractor> logger, IHttpClientFactory clientFactory)
        {
            baseUrl = configuration["DataSources:Api:baseUrl"] ?? string.Empty;
            _logger = logger;
            _clientFactory = clientFactory;
            _configuracion = configuration;
        }

        public async Task<IEnumerable<ApiProduct>> ExtractAsync()
        {
            List<ApiProduct> Products = new List<ApiProduct>();
            try
            {
                using var client = _clientFactory.CreateClient("ApiProductClient");
                client.BaseAddress = new Uri(baseUrl);

                using var response = await client.GetAsync("api/Product/get-products");

                if (response.IsSuccessStatusCode)
                {
                    var apiProducts = await response.Content.ReadFromJsonAsync<IEnumerable<ApiProduct>>();
                    if (apiProducts != null)
                    {
                        Products = apiProducts.ToList();
                    }
                }
                else
                {
                    _logger.LogError("Error al buscar productos en la API. Status Code {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al realizar el proceso de extracción de Productos de la API");
            }
            return Products;
        }
    }
}