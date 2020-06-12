using Mango.RabbitMQ.Util;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        //ISingletonDependency
        static IConnectionPool connectionPool;
        static IChannelPool channelPool;
        //ITransientDependency
        static IRabbitMqSerializer serializer;
        static IChannelAccessor ChannelAccessor;

        private const string ChannelQueueName = "MQ.Zhh.Test";
        private const string ConnectionName = "Console";
        private const string message = "Hello World!";
        protected static EventingBasicConsumer Consumer;
        static void Main(string[] args)
        {
            serializer = new Utf8JsonRabbitMqSerializer();
            connectionPool = new ConnectionPool(new RabbitMqOptions());
            channelPool = new ChannelPool(connectionPool);

            ChannelAccessor = channelPool.Acquire(ChannelQueueName, ConnectionName);


            QueueDeclareConfiguration QueueConfiguration = new QueueDeclareConfiguration(ChannelQueueName);
            QueueConfiguration.Declare(ChannelAccessor.Channel);

            var properties = ChannelAccessor.Channel.CreateBasicProperties();
            properties.Persistent = true;

            ChannelAccessor.Channel.BasicPublish(
               exchange: "",
               routingKey: ChannelQueueName,
               basicProperties: properties,
               body: serializer.Serialize(message)
           );


            Consumer = new EventingBasicConsumer(ChannelAccessor.Channel);
            Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = (string)serializer.Deserialize(ea.Body.ToArray(), typeof(string));
                Console.WriteLine(" [1] Received {0}", message);
                ChannelAccessor.Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };


            ChannelAccessor.Channel.BasicConsume(
                   queue: ChannelQueueName,
                   autoAck: false,
                   consumer: Consumer
               );

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
}
