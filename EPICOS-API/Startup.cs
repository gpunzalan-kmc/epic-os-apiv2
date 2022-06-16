using EPICOS_API.Helpers;
using EPICOS_API.Managers;
using EPICOS_API.Models;
using Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPICOS_API
{
    public class Startup
    {
        public List<Type> TypesToRegister { get; }
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Environment = env;
            Configuration = configuration;
            DatabaseConnection.configuration = configuration;
        }
        readonly string AllowedSpecificOrigins = "_allowSpecificOrigins";

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy(AllowedSpecificOrigins,
                builder =>
                {
                    builder.SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("https://localhost:44333",
                                        "http://localhost:8081",
                                        "http://localhost:8080",
                                        "http://192.168.10.28:86",
                                        "https://epicos.kmc.solutions",
                                        "https://www.kmcmaggroup.com");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            services.AddHttpClient();
            services.AddSingleton<IConfiguration>(Configuration);
            var jwtSection = Configuration.GetSection("JWTSettings");
            services.Configure<Models.JWTSettings>(jwtSection);
            var appSettings = jwtSection.Get<JWTSettings>();
            var key = appSettings.SecretKey;

            var ospSection = Configuration.GetSection("OSPSettings");
            services.Configure<Models.OSPSettings>(ospSection);
            var ospSettings = ospSection.Get<OSPSettings>();
            var ospKey = ospSettings.SecretKey;
            var ospBaseURI = ospSettings.BaseURI;

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSingleton<IJwtAuthenticationManager>(new JwtAuthenticationManager(key));
            services.AddSingleton<IHttpCallManager>(new HttpCallManager(ospKey, ospBaseURI));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            app.UseCors(AllowedSpecificOrigins);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
      

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
