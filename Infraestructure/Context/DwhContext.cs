using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Dwh.Dimensions;
using Domain.Entities.Dwh.Facts;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Context
{
    public class DwhContext : DbContext
    {
        public DwhContext(DbContextOptions<DwhContext> options) : base(options) { }

        public DbSet<dimCustomer> dimCustomers { get; set; }
        public DbSet<dimProduct> dimProducts { get; set; }
        public DbSet<dimFecha> dimFecha { get; set; }
        public DbSet<FactVenta> factVentas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<dimCustomer>()
                .ToTable("dimCustomers", "Dimension");

            modelBuilder.Entity<dimProduct>()
                .ToTable("dimProducts", "Dimension");

            modelBuilder.Entity<dimFecha>()
            .ToTable("dimFecha", "Dimension");

            modelBuilder.Entity<FactVenta>()
                .ToTable("FactVentas", "Fact");

            base.OnModelCreating(modelBuilder);
        }
    }
}