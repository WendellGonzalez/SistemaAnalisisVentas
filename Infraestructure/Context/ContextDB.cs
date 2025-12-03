using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.DB;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Context
{
    public class ContextDB : DbContext
    {
        public ContextDB(DbContextOptions<ContextDB> options) : base(options) { }
        
        public DbSet<VentaDB> HistoricoVentas { get; set; }
    }
}