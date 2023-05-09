using System;
using InvoicesCRUD.Core.Domain.RepositoryContracts;
using InvoicesCRUD.Core.ServiceContracts;
using InvoicesCRUD.Core.Services;
using InvoicesCRUD.Infrastructure.DatabaseContext;
using InvoicesCRUD.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InvoicesCRUD.UI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllersWithViews();

            services.AddScoped<IInvoicesRepository, InvoicesRepository>();
            services.AddScoped<IInvoiceProductsRepository, InvoiceProductsRepository>();
            services.AddScoped<IRunningNumbersRepository, RunningNumbersRepository>();
            services.AddScoped<IInvoicesService, InvoicesService>();
            services.AddScoped<IInvoiceProductsService, InvoiceProductsService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString(
                    "DefaultConnection"));
            });

            return services;
        }
    }
}

