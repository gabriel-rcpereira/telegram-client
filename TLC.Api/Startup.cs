﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using Quartz.Impl;
using System.Diagnostics;
using System.IO;
using TLC.Api.Configurations.Telegram;
using TLC.Api.Factories;
using TLC.Api.Factories.Contracts;
using TLC.Api.Helpers;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Jobs;
using TLC.Api.Jobs.Factories;
using TLC.Api.Models.Mappers;
using TLC.Api.Services;
using TLC.Api.Services.Contracts;

namespace TLC.Api
{
    public class Startup
    {
        private const string TelegramAppConfiguration = "Telegram";

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .AddApiExplorer()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            // configurations
            services.Configure<TelegramConfiguration>(_configuration.GetSection($"{TelegramAppConfiguration}"));
            // logging
            services.AddLogging(logging => logging.AddFile(GetFileLogPath(), LogLevel.Information));

            ConfigureDepencyInjection(services);
            ConfigureSchedule(services.BuildServiceProvider());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TelegramApp Client", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TelegramApi");
            });
            app.UseMvc();
        }

        private static IMapper ConfigureMapper()
        {            
            var mapperConfiguration = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new ClientMapperProfile());
                configuration.AddProfile(new TelegramMapperProfile());
                configuration.AddProfile(new ContactMapperProfile());
            });
            return mapperConfiguration.CreateMapper();
        }

        private static void ConfigureDepencyInjection(IServiceCollection services)
        {
            // di
            services.AddTransient<ITelegramClientFactory, TelegramClientFactory>();
            services.AddTransient<ITelegramHelper, TelegramHelper>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<INewService, NewsService>();
            // job
            services.AddTransient<NewsJob>();
            // mapper
            services.AddSingleton(ConfigureMapper());
        }

        private static void ConfigureSchedule(ServiceProvider serviceProvider)
        {
            var scheduler = new StdSchedulerFactory().GetScheduler()
                .Result;

            var trigger = CreateTrigger();

            scheduler.ScheduleJob(
                JobBuilder.Create<NewsJob>()
                    .WithIdentity(typeof(NewsJob).Name, "telegramGroup")
                    .Build(),
                trigger);
            scheduler.JobFactory = new JobFactory(serviceProvider);
            scheduler.Start();
        }

        private static ITrigger CreateTrigger()
        {
            return TriggerBuilder.Create()
                            .WithIdentity("newsTrigger", "telegramGroup")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithIntervalInSeconds(45)
                                .RepeatForever())
                            .Build();
        }

        private string GetFileLogPath()
        {
            var logFileTxt = "tlcApi.log";

            if (Debugger.IsAttached)
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                return (currentDirectory.LastIndexOf(Path.DirectorySeparatorChar) == currentDirectory.Length - 1 ?
                    currentDirectory :
                    currentDirectory + Path.DirectorySeparatorChar) + logFileTxt;
            }
            else
            {
                return $@"C:\ProgramData\Temp\{logFileTxt}";
            }
        }
    }
}
