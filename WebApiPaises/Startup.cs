using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiPaises.Models;

namespace WebApiPaises
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
            //START:ESTA CONFIGURACION ES PARA USAR UNA BASE DE DATOS TEMPORAL
            //services.AddDbContext<ApplicationDbContext>(options => 
            //options.UseInMemoryDatabase("paisDB"));
            //services.AddMvc().AddJsonOptions(ConfigureJson);
            //END:ESTA CONFIGURACION ES PARA USAR UNA BASE DE DATOS TEMPORAL

            //START:ESTA CONFIGURACION ES PARA USAR UNA BASE DE DATOS DE SQL SERVER
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
            //END:ESTA CONFIGURACION ES PARA USAR UNA BASE DE DATOS DE SQL SERVER

            //IMPLEMENTACION DE SEGURIDAD BASICA(HTTP ERROR 401,Esta página no funciona)
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            //START:AGREGAR SEGURIDAD AL WEB API
             services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourdomain.com",
                    ValidAudience = "yourdomain.com",
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["Llave_super_secreta"])),
                    ClockSkew = TimeSpan.Zero
                });
            //END:AGREGAR SEGURIDAD AL WEB API

            services.AddMvc().AddJsonOptions(ConfigureJson);
        }

        private void ConfigureJson(MvcJsonOptions obj)
        {
            obj.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();

            //ESTO ES PARA TENER DATA EN UNA BASE DE DATOS TEMPORAL
            //if (!context.Paises.Any())
            //{
            //    context.Paises.AddRange(new List<Pais>()
            //    {
            //        new Pais(){Nombre = "República Dominicana", Provincias = new List<Provincia>(){
            //                new Provincia(){Nombre = "Azua"}
            //        } },
            //        new Pais(){Nombre = "México", Provincias = new List<Provincia>(){
            //                new Provincia(){Nombre = "Puebla"},
            //                new Provincia(){Nombre = "Queretaro"}
            //            } },
            //        new Pais(){Nombre = "Argentina"}
            //    });

            //    context.SaveChanges();
            //}

        }
    }
}
