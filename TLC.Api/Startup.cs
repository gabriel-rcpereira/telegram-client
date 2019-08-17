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
using TLC.Api.Services;
using TLC.Api.Services.Contracts;

namespace TLC.Api
{
    public class Startup
    {
        private const string TelegramConfiguration = "Telegram";
        private const string ClientConfiguration = "Client";

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
            services.Configure<Client>(_configuration.GetSection($"{TelegramConfiguration}::{ClientConfiguration}"));

            ConfigureDepencyInjection(services);
            ConfigureSchedule(services);
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

        private static void ConfigureDepencyInjection(IServiceCollection services)
        {
            // DI
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<ITelegramHelper, TelegramHelper>();
            // job
            services.AddTransient<NewsJob>();
        }

        private static void ConfigureSchedule(IServiceCollection services)
        {
            var scheduler = new StdSchedulerFactory().GetScheduler()
                .Result;
            scheduler.ScheduleJob(JobBuilder.Create<NewsJob>().Build(), 
                CreateTrigger());
            scheduler.JobFactory = new JobFactory(services.BuildServiceProvider());
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
    }
}
