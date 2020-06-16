using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Mango.RabbitMQ.Util.Module
{
    public class RabbitMQMiddleware : IMiddleware
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
        private const string message = "Hello World!{0}";

        public RabbitMQMiddleware(
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
                Console.WriteLine(" [1] Received {0}", message);
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

            return next.Invoke(context);
            //throw new NotImplementedException();
        }

    }
    public static class RabbitMQMiddlewareExtension
    {
        public static void AddRabbitMQMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<IChannelPool, ChannelPool>();
            services.AddSingleton<IConnectionPool, ConnectionPool>();
            services.AddSingleton<IRabbitMqMessageConsumerFactory, RabbitMqMessageConsumerFactory>();
            services.AddTransient<IRabbitMqMessageConsumer, RabbitMqMessageConsumer>();
            services.AddTransient<IRabbitMqSerializer, Utf8JsonRabbitMqSerializer>();
        }
    }
}
