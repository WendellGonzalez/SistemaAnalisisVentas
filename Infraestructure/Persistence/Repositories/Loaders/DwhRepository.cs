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

        public async Task<ServiceResponse> LoadDimData(DimCollection dimCollection)
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

                var customers = await dimCollection._customersCollection.GetAllCustomersAsync();
                var products = await dimCollection._productCollection.GetAllProductsAsync();
                var fechas = await dimCollection._fechasCollection.GetAllFechasAsync();

                if(!customers.Any() || !products.Any() || !fechas.Any())
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "Las colecciones de dimensiones estan vacias"
                    };
                }

                var dimCustomers = customers.Select(c => new dimCustomer
                {
                    CustomerID = c.CustomerID,
                    FirstName = c.FirstName!,
                    LastName = c.LastName!,
                    City = c.City!,
                    Country = c.Country!
                }).ToList();

                var dimProducts = products.Select(p => new dimProduct
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Category = p.Category,
                    Price = p.Price
                }).ToList();

                var dimFechas = fechas.Select(f => new dimFecha
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
                });

                await _context.factVentas.ExecuteDeleteAsync();
                await _context.dimFecha.ExecuteDeleteAsync();
                await _context.dimProducts.ExecuteDeleteAsync();
                await _context.dimCustomers.ExecuteDeleteAsync();

                _context.BulkInsert(dimFechas);
                _context.BulkInsert(dimCustomers);
                _context.BulkInsert(dimProducts);

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

        public async Task<ServiceResponse> LoadFactData(FactCollection factCollection, DimCollection dimCollection)
        {
            try
            {
                if (factCollection._ventasCollection == null)
                {
                    return new ServiceResponse
                    {
                        Success = false,
                        Message = "La lista de ventas está vacía"
                    };
                }

                var ventas = await factCollection._ventasCollection.GetAllVentasAsync();
                var productsDTO = await dimCollection._productCollection.GetAllProductsAsync();

                var factVentas = ventas.Select(v => new FactVenta()
                {
                    OrderID = v.OrderID,
                    ProductID = v.ProductID,
                    CustomerID = v.CustomerID,
                    FechaID = int.Parse(v.OrderDate.ToString("yyyyMMdd")),
                    Quantity = v.Quantity,
                    TotalPrice = v.TotalPrice

                }).ToList();

                _context.BulkInsert(factVentas);

                return new ServiceResponse
                {
                    Success = true,
                    Message = $"Insertadas {factVentas.Count} ventas."
                };

            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = "Error cargando fact: " + e.Message
                };
            }
        }
    }
}