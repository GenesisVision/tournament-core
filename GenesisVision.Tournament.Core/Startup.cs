﻿using GenesisVision.DataModel;
using GenesisVision.Tournament.Core.Infrastructure;
using GenesisVision.Tournament.Core.Services;
using GenesisVision.Tournament.Core.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;

namespace GenesisVision.Tournament.Core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["DbContextSettings:ConnectionString"];
            var dbContextOptions = new Action<NpgsqlDbContextOptionsBuilder>(options =>
                options.MigrationsAssembly("GenesisVision.Tournament.Core"));

            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(connectionString, dbContextOptions));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader()
                                      .AllowCredentials());
            });

            services.AddMemoryCache()
                    .AddMvcCore()
                    .AddApiExplorer()
                    .AddDataAnnotations()
                    .AddJsonFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";
                    });

            services.AddTransient<ITournamentService, TournamentService>();
            services.AddTransient<ITradeServerService, TradeServerService>();
            services.AddTransient<IStatisticService, StatisticService>();
            services.AddTransient<IIpfsService, IpfsService>();

            Constants.IpfsHost = Configuration["IpfsHost"];

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                                   {
                                       Title = "Tournament Core API",
                                       Version = "v1",
                                       Contact = new Contact
                                                 {
                                                     Name = "Genesis Vision",
                                                     Url = "https://genesis.vision/"
                                                 }
                                   });
                c.DescribeAllEnumsAsStrings();
                c.TagActionsBy(x => x.RelativePath.Split("/").Take(2).Last());
                c.OperationFilter<FileUploadOperation>();

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "GenesisVision.Tournament.Core.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || bool.Parse(Configuration["IsDevelopment"]))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tournament Core API v1");
            });
        }
    }
}
