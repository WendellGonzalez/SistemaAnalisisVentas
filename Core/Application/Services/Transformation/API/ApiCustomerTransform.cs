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
    public class ApiCustomerTransform : ICustomerSource
    {
        private readonly IExtractor<ApiCustomer> _apiCustomers;

        private readonly ILogger<ApiCustomerTransform> _logger;

        public ApiCustomerTransform(IExtractor<ApiCustomer> apiCustomers, ILogger<ApiCustomerTransform> logger)
        {
            _apiCustomers = apiCustomers;
            _logger = logger;
        }
        public async Task<List<Customer>> GetCleanCustomersAsync()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                var customersApi = await _apiCustomers.ExtractAsync();

                customers.AddRange(customersApi.Select(c => new Customer()
                {
                    CustomerID = c.CustomerID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    City = c.City,
                    Country = c.Country,
                    DataSource = "API"
                }));

                return customers;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer customers de Api limpios");
                throw;
            }
        }
    }
}