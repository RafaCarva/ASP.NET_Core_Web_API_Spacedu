﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MimicAPI.Database;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Contracts;
using AutoMapper;
using MimicAPI.Helpers;
using Microsoft.AspNetCore.Mvc.Versioning;

using System.IO;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Automapper - Configuração
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            // DB
            services.AddDbContext<MimicContext>(opt =>
                {
                    opt.UseSqlite("Data Source=Database\\Mimic.db");
                }

            );
            services.AddMvc();
            services.AddScoped<IPalavraRepository, PalavraRepository>();

            // Adicionar versionamento
            services.AddApiVersioning(cfg =>
              {
                  cfg.ReportApiVersions = true;
                  cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Add msgs nos erros
            app.UseStatusCodePages();

           // app.UseMvc();
      
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
          
            
        }
    }
}
