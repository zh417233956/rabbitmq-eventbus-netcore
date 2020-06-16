using Mango.RabbitMQ.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Publish.App.Module
{
    public class RabbitMQPublishMiddleware : IMiddleware
    {
        //ISingletonDependency
        //IConnectionPool connectionPool;
        IChannelPool channelPool;
        //ITransientDependency
        IRabbitMqSerializer serializer;
        IChannelAccessor ChannelAccessor;

        protected EventingBasicConsumer Consumer;
        private const string ChannelQueueName = "MQ.Zhh.Test";
        private const string ConnectionName = "Consume";
        private const string message = "Hello World!-{0}";

        public RabbitMQPublishMiddleware(
            IChannelPool channelPool,
            IRabbitMqSerializer serializer)
        {
            this.serializer = serializer;
            this.channelPool = channelPool;
            ChannelAccessor = channelPool.Acquire(ChannelQueueName, ConnectionName);

        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {            
            QueueDeclareConfiguration QueueConfiguration = new QueueDeclareConfiguration(ChannelQueueName);
            QueueConfiguration.Declare(ChannelAccessor.Channel);

            var properties = ChannelAccessor.Channel.CreateBasicProperties();
            properties.Persistent = true;

            ChannelAccessor.Channel.BasicPublish(
                exchange: "",
                routingKey: QueueConfiguration.QueueName,
                basicProperties: properties,
                body: serializer.Serialize(string.Format(message, DateTime.Now))
            );           
            Console.WriteLine(" [Publish] Publish {0}", string.Format(message,DateTime.Now));
            return next.Invoke(context);
        }

    }
    public static class RabbitMQPublishMiddlewareExtension
    {
        public static void AddRabbitMQPublishMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<IChannelPool, ChannelPool>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
            services.AddTransient<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();
            services.AddTransient<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
            //中间件注入
            services.AddSingleton<RabbitMQPublishMiddleware>();
        }
        public static IApplicationBuilder UseRabbitMQPublishMiddleware(this IApplicationBuilder builder)
        {
            // 使用UseMiddleware将自定义中间件添加到请求处理管道中
            return builder.UseMiddleware<RabbitMQPublishMiddleware>();
        }
    }
}
