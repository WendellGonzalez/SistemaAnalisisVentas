using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Services.DwhCollections;
using Domain.Entities.Dwh.Dimensions;
using Domain.Entities.Dwh.Facts;
using EFCore.BulkExtensions;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories.Loaders
{
    public class DwhRepository : IDwhRepository
    {
        private readonly DwhContext _context;

        public DwhRepository(DwhContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse> LoadData(DimCollection dimCollection, FactCollection factCollection)
        {
            try
            {
                if (dimCollection._customersCollection == null ||
                    dimCollection._productCollection == null ||
                    dimCollection._fechasCollection == null)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Las colecciones de dimensiones vienen NULL."
                    };
                }

                var customersDTO = await dimCollection._customersCollection.GetAllCustomersAsync();
                var productsDTO = await dimCollection._productCollection.GetAllProductsAsync();
                var fechasDTO = await dimCollection._fechasCollection.GetAllFechasAsync();
                var ventasDTO = await factCollection._ventasCollection.GetAllVentasAsync();

                if (!customersDTO.Any() || !productsDTO.Any() || !fechasDTO.Any())
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Las colecciones de dimensiones estan vacias"
                    };
                }

                var Customers = customersDTO
                .GroupBy(c => c.CustomerID)
                .Select(c => c.First())
                .Select(c => new dimCustomer
                {
                    CustomerID = c.CustomerID,
                    FirstName = c.FirstName!,
                    LastName = c.LastName!,
                    City = c.City!,
                    Country = c.Country!
                }).ToList();

                var Products = productsDTO
                .GroupBy(p => p.ProductID)
                .Select(p => p.First())
                .Select(p => new dimProduct
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price
                }).ToList();

                var Fechas = fechasDTO.Select(f => new dimFecha
                {
                    FechaID = int.Parse(f.ToString("yyyyMMdd")),
                    Fecha = f,
                    Anio = f.Year,
                    Mes = f.Month,
                    NombreMes = f.ToString("MMMM", new CultureInfo("es-ES")),
                    Dia = f.Day,
                    NombreDia = f.ToString("dddd", new CultureInfo("es-ES")),
                    Trimestre = (f.Month - 1) / 3 + 1,
                    Cuatrimestre = (f.Month - 1) / 4 + 1,
                    Semestre = (f.Month - 1) / 6 + 1,
                    Semana = ISOWeek.GetWeekOfYear(f)
                }).ToList();

                await _context.factVentas.ExecuteDeleteAsync();
                await _context.dimFecha.ExecuteDeleteAsync();
                await _context.dimProducts.ExecuteDeleteAsync();
                await _context.dimCustomers.ExecuteDeleteAsync();



                await _context.BulkInsertAsync(Fechas);
                await _context.BulkInsertAsync(Customers);
                await _context.BulkInsertAsync(Products);

                var dimProducts = await _context.dimProducts.ToListAsync();
                var dimCustomers = await _context.dimCustomers.ToListAsync();
                var dimFechas = await _context.dimFecha.ToListAsync();

                var factVentas = ventasDTO
                .GroupBy(v => new {v.OrderID, v.ProductID, v.CustomerID, v.OrderDate})
                .Select(g => g.First())
                .Select(v => new FactVenta()
                {
                    OrderID = v.OrderID,
                    ProductKey = dimProducts.FirstOrDefault(p => p.ProductID == v.ProductID).ProductKey,
                    CustomerKey = dimCustomers.FirstOrDefault(c => c.CustomerID == v.CustomerID).CustomerKey,
                    FechaKey = int.Parse(v.OrderDate.ToString("yyyyMMdd")),
                    Quantity = v.Quantity,
                    TotalPrice = v.TotalPrice

                }).ToList();

                await _context.BulkInsertAsync(factVentas);

                return new ServiceResponse
                {
                    Success = true,
                    Message = $"Insertados {dimCustomers.Count} customers y {dimProducts.Count} products."
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Error cargando dimensiones: " + e.Message
                };
            }
        }
    }
}