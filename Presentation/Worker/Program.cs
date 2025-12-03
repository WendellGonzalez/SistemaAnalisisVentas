using Application.Interfaces.Collections;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services.Collections;
using Application.Services.Transformation.API;
using Application.Services.Transformation.CSV;
using Application.Services.Transformation.DB;
using Domain.Entities.API;
using Domain.Entities.CSV;
using Domain.Entities.DB;
using Infraestructure.Context;
using Infraestructure.Persistence.Repositories.Extractors.API;
using Infraestructure.Persistence.Repositories.Extractors.CSV;
using Infraestructure.Persistence.Repositories.Extractors.DB;
using Infraestructure.Persistence.Repositories.Loaders;
using Microsoft.EntityFrameworkCore;
using Worker;

class Program
{
    static void Main(string[] args)
    {
        // var builder = Host.CreateApplicationBuilder(args);
        // builder.Services.AddHostedService<Worker>();

        // var host = builder.Build();
        // host.Run();

        var builder = Host.CreateApplicationBuilder(args);

        // Inyeccion de la conexion de la Db transaccional
        builder.Services.AddSqlServer<ContextDB>(builder.Configuration.GetConnectionString("OperationalDB"));

        // Inyeccion de la conexion de la Dwh 
        builder.Services.AddSqlServer<DwhContext>(builder.Configuration.GetConnectionString("DataWarehouse"));

        // SERVICE INYECCIONS

        // Inyecciones de ExtractionRepositories de APIs 
        builder.Services.AddScoped<IExtractor<ApiCustomer>, ApiCustomerExtractor>();
        builder.Services.AddScoped<IExtractor<ApiProduct>, ApiProductExtractor>();

        // Inyecciones de ExtractionRepositories de Csvs
        builder.Services.AddScoped<IExtractor<CsvCustomer>, CsvCustomerExtractor>();
        builder.Services.AddScoped<IExtractor<CsvOrderDetail>, CsvOrderDetailExtractor>();
        builder.Services.AddScoped<IExtractor<CsvOrder>, CsvOrderExtractor>();
        builder.Services.AddScoped<IExtractor<CsvProduct>, CsvProductExtractor>();

        // Inyecciones de extracciones de DB
        builder.Services.AddScoped<IExtractor<VentaDB>, DbVentasExtractor>();

        // Sources
        builder.Services.AddScoped<ICustomerSource, ApiCustomerTransform>();
        builder.Services.AddScoped<ICustomerSource, CsvCustomerTransform>();
        builder.Services.AddScoped<IProductSource, ApiProductTransform>();
        builder.Services.AddScoped<IProductSource, CsvProductTransform>();
        builder.Services.AddScoped<IVentaSource, CsvVentasTransform>();
        builder.Services.AddScoped<IVentaSource, DbVentasTransform>();

        builder.Services.AddScoped<ICustomersCollection, CustomersCollection>();
        builder.Services.AddScoped<IProductsCollection, ProductsCollection>();
        builder.Services.AddScoped<IFechasCollection, FechasCollection>();
        builder.Services.AddScoped<IVentasCollection, VentasCollection>();

        builder.Services.AddScoped<IDwhRepository, DwhRepository>();

        // HttpClient para API Extractors
        builder.Services.AddHttpClient();

        builder.Services.AddHostedService<Worker.Worker>();

        var host = builder.Build();
        host.Run();
    }
}
