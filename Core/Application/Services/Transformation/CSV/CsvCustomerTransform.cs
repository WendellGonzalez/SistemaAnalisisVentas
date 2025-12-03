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
    public class CsvCustomerTransform : ICustomerSource
    {
        private readonly IExtractor<CsvCustomer> _csvCustomers;
        private readonly ILogger<CsvCustomerTransform> _logger;

        public CsvCustomerTransform(IExtractor<CsvCustomer> csvCustomers, ILogger<CsvCustomerTransform> logger)
        {
            _csvCustomers = csvCustomers;
            _logger = logger;
        }
        public async Task<List<Customer>> GetCleanCustomersAsync()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                var customersCsv = await _csvCustomers.ExtractAsync();
                
                customers.AddRange(customersCsv.Select(c => new Customer()
                {
                    CustomerID = c.CustomerID,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    City = c.City,
                    Country = c.Country
                }));

                return customers;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer Customers de Csv limpios");
                throw;
            }
        }
    }
}