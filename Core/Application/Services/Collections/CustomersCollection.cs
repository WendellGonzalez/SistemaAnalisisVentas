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
    public class CustomersCollection : ICustomersCollection
    {
        private readonly IEnumerable<ICustomerSource> _customersSource;
        private readonly ILogger<CustomersCollection> _logger;

        public CustomersCollection(IEnumerable<ICustomerSource> customersSource, ILogger<CustomersCollection> logger)
        {
            _customersSource = customersSource;
            _logger = logger;
        }
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                var allCustomers = new List<Customer>();

                var tasks = _customersSource.Select(source => source.GetCleanCustomersAsync());
                var results = await Task.WhenAll(tasks);

                foreach (var customers in results)
                {
                    allCustomers.AddRange(customers);
                }

                return allCustomers
                .OrderBy(o => o.CustomerID)
                .GroupBy(c => c.CustomerID)
                .Select(g => g.First())
                .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al traer los customers anidados");
                throw;
            }
        }
    }
}