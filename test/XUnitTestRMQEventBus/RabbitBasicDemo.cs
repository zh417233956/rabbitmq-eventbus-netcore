using Mango.RabbitMQ.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestRMQEventBus
{
    public class RabbitBasicDemo
    {
        ILogger<RabbitBasicDemo> _logger;
        IChannelPool channelPool;
        IRabbitMqSerializer serializer;
        IChannelAccessor ChannelAccessor;

        protected EventingBasicConsumer Consumer;
        private const string ChannelQueueName = "MQ.Zhh.XUnit";
        private const string ConnectionName = "Consume";

        public RabbitBasicDemo(
            IChannelPool channelPool,
            IRabbitMqSerializer serializer,
            ILogger<RabbitBasicDemo> logger)
        {
            _logger = logger;
            this.serializer = serializer;
            this.channelPool = channelPool;
            ChannelAccessor = channelPool.Acquire(ChannelQueueName, ConnectionName);

        }
        public void Publish(string message)
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
            _logger.LogInformation(" [RabbitBasicDemo] Publish {0}", string.Format(message, DateTime.Now));
        }

        public void Subscribe()
        {
            QueueDeclareConfiguration QueueConfiguration = new QueueDeclareConfiguration(ChannelQueueName);
            QueueConfiguration.Declare(ChannelAccessor.Channel);

            var properties = ChannelAccessor.Channel.CreateBasicProperties();
            properties.Persistent = true;



            Consumer = new EventingBasicConsumer(ChannelAccessor.Channel);
            Consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                string message = (string)serializer.Deserialize(ea.Body.ToArray(), typeof(string));
                _logger.LogInformation(" [Subscribe] Received {0}", string.Format(message, DateTime.Now));
                ChannelAccessor.Channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            ChannelAccessor.Channel.BasicConsume(
                   queue: QueueConfiguration.QueueName,
                   autoAck: false,
                   consumer: Consumer
               );
        }
    }

}
