using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Core
{
    public static class RabbitMQIocExtension
    {
        public static void AddRabbitMQ(this IServiceCollection services)
        {
            services.AddSingleton<IChannelPool, ChannelPool>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
            services.AddTransient<RabbitMqMessageConsumer>();
            services.AddTransient<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
        }
    }
}
