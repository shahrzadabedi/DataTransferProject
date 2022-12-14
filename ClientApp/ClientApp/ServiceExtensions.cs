using ClientApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TeamContext>(opts =>
            //opts.UseMemoryCache(IMemory));
            opts.UseSqlServer(configuration.GetConnectionString("teamConnection"), b => b.MigrationsAssembly("ClientApp.Infrastructure")));           
        }
        public static void ConfigureOtherContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TeamContext>(opts =>
               opts.UseSqlServer(configuration.GetConnectionString("teamConnection"), b => b.MigrationsAssembly("ClientApp.Infrastructure")));
        }
    }
}
