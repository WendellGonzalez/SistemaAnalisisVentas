using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces.Collections
{
    public interface ICustomersCollection
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
    }
}