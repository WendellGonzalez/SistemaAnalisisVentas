using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Collections;
using Application.Interfaces.Repositories;
using Application.Services.DwhCollections;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public static class CollectionService
    {
        private static async Task<ServiceResponse> ExecuteAllDataLoading(
                                                IDwhRepository repository, 
                                                ICustomersCollection customersCollection, 
                                                IProductsCollection productCollection, 
                                                IFechasCollection fechasCollection 
                                                ,IVentasCollection ventasCollection
                                                )
        {
            DimCollection dimCollection = new DimCollection(customersCollection, productCollection, fechasCollection);
            FactCollection factCollection = new FactCollection(ventasCollection);
            try
            {
                var dim = await repository.LoadData(dimCollection, factCollection);
                // var fact = await repository.LoadFactData(factCollection, dimCollection);

                return new ServiceResponse
                {
                    Success = true,
                    Message = "Carga de datos realizada con exito"
                };
            }
            catch (Exception e)
            {
                return new ServiceResponse
                {
                    Success = false,
                    Message = $"Error en el proceso ETL: {e}"
                };

                throw;
            }
        }

        public static async Task<ServiceResponse> ExectuteETLProcess(IServiceProvider _serviceProvider)
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IDwhRepository>();
            var customersCollection = scope.ServiceProvider.GetRequiredService<ICustomersCollection>();
            var productCollection = scope.ServiceProvider.GetRequiredService<IProductsCollection>();
            var ventasCollection = scope.ServiceProvider.GetRequiredService<IVentasCollection>();
            var fechasCollection = scope.ServiceProvider.GetRequiredService<IFechasCollection>();

            var ETL = await ExecuteAllDataLoading(
                            repository,
                            customersCollection,
                            productCollection,
                            fechasCollection
                            ,ventasCollection
                            );

            return ETL;
        }
    }
}