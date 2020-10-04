using System;
using System.Collections.Generic;
using System.Linq;
using CyberPets.Infrastructure;
using CyberPets.Domain;
using CyberPets.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CyberPets.API
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
            services.AddControllers();

            services.AddSingleton<IUserPetsRepository, UserPetsInMemoryRepository>();
            services.AddSingleton<ITimeProvider, SystemTimeProvider>();
            services.AddSingleton<PetKinds>(new PetKinds(PetKind.KnownKinds));
            services.AddSingleton<UserPetsService>();

            // Add Swashbuckle swagger generator for live documentation
            services.AddMvc();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Add Swashbuckle swagger generator for live documentation
            app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CyberPets.Api"));
        }
    }
}
