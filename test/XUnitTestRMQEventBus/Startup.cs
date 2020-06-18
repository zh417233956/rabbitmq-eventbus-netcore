using Mango.RabbitMQ.Core;
using Mango.RabbitMQ.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestRMQEventBus
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
            services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMQ"));
            services.Configure<Mango.RabbitMQ.EventBus.RabbitMQ.RabbitMqEventBusOptions>(Configuration.GetSection("RabbitMQ:EventBus"));

            services.AddRMQEventBus();
            services.AddSingleton<RabbitBasicDemo>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();
        }
    }
}
