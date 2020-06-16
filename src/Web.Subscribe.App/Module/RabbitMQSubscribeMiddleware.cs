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

namespace Web.Subscribe.App.Module
{
    public class RabbitMQSubscribeMiddleware : IMiddleware
    {
        IChannelPool channelPool;
        IRabbitMqSerializer serializer;
        IChannelAccessor ChannelAccessor;

        protected EventingBasicConsumer Consumer;
        private const string ChannelQueueName = "MQ.Zhh.Test";
        private const string ConnectionName = "Consume";
        private const string message = "Hello World!-{0}";

        public RabbitMQSubscribeMiddleware(
            IChannelPool channelPool,
            IRabbitMqSerializer serializer)
        {
            this.serializer = serializer;
            this.channelPool = channelPool;

            ChannelAccessor = channelPool.Acquire(ChannelQueueName, ConnectionName);


            QueueDeclareConfiguration QueueConfiguration = new QueueDeclareConfiguration(ChannelQueueName);
            QueueConfiguration.Declare(ChannelAccessor.Channel);

            var properties = ChannelAccessor.Channel.CreateBasicProperties();
            properties.Persistent = true;



            Consumer = new EventingBasicConsumer(ChannelAccessor.Channel);
            Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = (string)serializer.Deserialize(ea.Body.ToArray(), typeof(string));
                Console.WriteLine(" [Subscribe] Received {0}", string.Format(message, DateTime.Now));
                ChannelAccessor.Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            ChannelAccessor.Channel.BasicConsume(
                   queue: QueueConfiguration.QueueName,
                   autoAck: false,
                   consumer: Consumer
               );

        }
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return next.Invoke(context);
        }

    }
    public static class RabbitMQSubscribeMiddlewareExtension
    {
        public static void AddRabbitMQSubscribeMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<IChannelPool, ChannelPool>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
            services.AddTransient<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();
            services.AddTransient<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
            //中间件注入
            services.AddSingleton<RabbitMQSubscribeMiddleware>();
        }
        public static IApplicationBuilder UseRabbitMQSubscribeMiddleware(this IApplicationBuilder builder)
        {
            // 使用UseMiddleware将自定义中间件添加到请求处理管道中
            return builder.UseMiddleware<RabbitMQSubscribeMiddleware>();
        }
    }
}
