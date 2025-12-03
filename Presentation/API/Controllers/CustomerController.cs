using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get-customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.ViewCustomers.ToListAsync();

                if (!customers.Any())
                {
                    _logger.LogError("Lista de clientes está vacia");
                    return NoContent();
                }
                return Ok(customers);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,"Error al obtener Clientes");
                return null!;
            }
        }
    }
}