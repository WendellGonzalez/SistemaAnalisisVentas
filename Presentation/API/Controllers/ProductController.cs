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
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("get-products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.ViewProducts.ToListAsync();
                if (!products.Any())
                {
                    _logger.LogError("Lista de productos está vacia");
                    return NoContent();
                }
                return Ok(products);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, "Error al obtener productos");
                return null!;
            }

        }
    }
}