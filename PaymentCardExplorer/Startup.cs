﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentCardExplorer.Interceptors;
using PaymentCardExplorer.ServiceRegistry;
using Persistence;
using Swashbuckle.AspNetCore.Swagger;
using Utilities.Binders;

namespace PaymentCardExplorer
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Configure CORs Policy based on access requirements
            services.AddCors(options =>
                options.AddPolicy("CorsPolicy",
                    p => p.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader()));

            //Register swagger
            services.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new Info()
                {
                    Title = "Payment Card Explorer",
                    Version = "V1",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact() { Email = "ediala94@gmail.com", Name = "Diala Emmanuel" }
                });
            });

            //Register database context
            services.AddDbContext<DatabaseContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("ApplicationDB"), b => b.MigrationsAssembly("Persistence")));

            //Register AppSetting Binders
            services.Configure<BaseUrls>(Configuration.GetSection("BaseUrls"));
            services.Configure<AuthTokens>(Configuration.GetSection("AuthTokens"));
            services.Configure<BrokerConfig>(Configuration.GetSection("BrokerConfig"));

            //Register service extensions
            services.RegisterServices();

            //Add Response caching 
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptions<AuthTokens> authTokens)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //add swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Payment Card Explorer");
            });

            //Configure Response caching
            app.UseResponseCaching();

            app.UseCors("CorsPolicy");

            //add request interceptor
            app.UseMiddleware<Authorization>(authTokens);

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
