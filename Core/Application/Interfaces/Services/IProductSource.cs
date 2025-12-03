using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IProductSource
    {
        Task<List<Product>> GetCleanProductsAsync();
    }
}