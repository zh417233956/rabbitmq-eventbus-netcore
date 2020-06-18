using Mango.RabbitMQ.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus.RabbitMQ
{
    public static class RMQEventBusIocExtension
    {
        public static void AddRMQEventBus(this IServiceCollection services)
        {
            //注入RabbitMQ
            services.AddRabbitMQ();
            //注入EventBus
            services.AddSingleton<IRabbitMQEventBus, RabbitMQEventBus>();
        }
    }
}
