using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using assistant_storekeeper_backend.Data;
using assistant_storekeeper_backend.Repositories.CompanyWarehouses;
using assistant_storekeeper_backend.Services.CompanyWarehouses;
using assistant_storekeeper_backend.Middlewares;
using assistant_storekeeper_backend.Repositories.Nomenclatures;
using assistant_storekeeper_backend.Services.Nomenclatures;
using assistant_storekeeper_backend.Repositories.Movements;
using assistant_storekeeper_backend.Services.Movements;
using assistant_storekeeper_backend.Repositories.MovementNomenclatures;
using assistant_storekeeper_backend.Services.MovementNomenclatures;

namespace assistant_storekeeper_backend
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
            string host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            string port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
            string dbName = Environment.GetEnvironmentVariable("POSTGRES_DATABASE");
            string user = Environment.GetEnvironmentVariable("POSTGRES_USERNAME");
            string pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
            string connectionString = $"Host={host};Port={port};Database={dbName};Username={user};Password={pass}";

            services.AddScoped<ICompanyWarehouseRepository, CompanyWarehouseRepository>();
            services.AddScoped<ICompanyWarehouseService, CompanyWarehouseService>();
            services.AddScoped<INomenclatureRepository, NomenclatureRepository>();
            services.AddScoped<INomenclatureService, NomenclatureService>();
            services.AddScoped<IMovementRepository, MovementRepository>();
            services.AddScoped<IMovementService, MovementService>();
            services.AddScoped<IMovementNomenclatureRepository, MovementNomenclatureRepository>();
            services.AddScoped<IMovementNomenclatureService, MovementNomenclatureService>();

            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
