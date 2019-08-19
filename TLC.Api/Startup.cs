using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using TLC.Api.Configuration.Telegram;
using TLC.Api.Helpers;
using TLC.Api.Helpers.Contracts;
using TLC.Api.Jobs;
using TLC.Api.Models.Mappers;
using TLC.Api.Services;
using TLC.Api.Services.Contracts;

namespace TLC.Api
{
    public class Startup
    {
        private const string TelegramAppConfiguration = "Telegram";

        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
                .AddJsonFormatters()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);

            // configurations
            services.Configure<TelegramConfiguration>(_configuration.GetSection($"{TelegramAppConfiguration}"));

            ConfigureDepencyInjection(services);
            ConfigureSchedule(services.BuildServiceProvider());            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
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
            // DI
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<ITelegramHelper, TelegramHelper>();
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
            //scheduler.Start();
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
    }
}
