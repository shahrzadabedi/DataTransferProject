using AutoMapper;
using ClientApp.Infrastructure;
using DataTransferLib;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using Microsoft.AspNetCore.Http;
using Quartz;
using ClientApp.API;

namespace ClientApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureSqlContext(Configuration);
            services.AddScoped<ITransferManager, TransferManager>();

            services.AddScoped<IConnectionMultiplexer>(sp =>
              ConnectionMultiplexer.Connect(new ConfigurationOptions
              {
                  EndPoints = { $"{Configuration.GetValue<string>("RedisCache:Host")}:{Configuration.GetValue<int>("RedisCache:Port")}" },
                  AbortOnConnectFail = false,
              }));

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new TeamProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddScoped<ISerializer, JSonSerializer>();
            services.AddScoped<IDBManager, SQLDBManager>();
            services.AddScoped<ICacheManager, RedisCacheManager>();
            services.AddScoped<IDomainDataConverter, DomainDataConverter>();
            services.AddScoped<ISourceDataReader, ExcelSourceDataReader>();
            services.AddScoped<IRepositoryWriter, RepositoryWriter>();
            services.AddScoped<IRepositoryReader, RepositoryReader>();
            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();

                var writeToRepositoryJob = new JobKey("WriteToRepositoryJob");
                q.AddJob<WriteToRepositoryJob>(opts => opts.WithIdentity(writeToRepositoryJob));
                q.AddTrigger(opts => opts
                    .ForJob(writeToRepositoryJob)
                    .WithIdentity("WriteToRepositoryJob-trigger")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(5)
                        .WithRepeatCount(1)));


            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();
            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(60)
                };
                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
                    new string[] { "Accept-Encoding" };

                await next();
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
