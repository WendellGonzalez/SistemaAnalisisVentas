using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using Domain.Entities.API;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.Extensions.Http;

namespace Infraestructure.Persistence.Repositories.Extractors.API
{
    public class ApiCustomerExtractor : IExtractor<ApiCustomer>
    {
        private readonly ILogger<ApiCustomerExtractor> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private string baseUrl;

        public ApiCustomerExtractor(IConfiguration configuration, ILogger<ApiCustomerExtractor> logger, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            baseUrl = configuration["DataSources:Api:baseUrl"]  ?? string.Empty;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        public async Task<IEnumerable<ApiCustomer>> ExtractAsync()
        {
            List<ApiCustomer> apiCustomers = new List<ApiCustomer>();
            try
            {
                using var client = _clientFactory.CreateClient("ApiCustomerClient");
                client.BaseAddress = new Uri(baseUrl);

                using var response = await client.GetAsync("api/Customer/get-customers");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<IEnumerable<ApiCustomer>>();
                    if (apiResponse != null)
                    {
                        apiCustomers = apiResponse.ToList();
                    }
                }
                else
                {
                    _logger.LogError("Error al buscar Customers en la API. Status code {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al realizar el proceso de extracción de Clientes de la API");
            }
            return apiCustomers!;
        }
    }
}