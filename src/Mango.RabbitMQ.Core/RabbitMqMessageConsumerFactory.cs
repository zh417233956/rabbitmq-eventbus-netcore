using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mango.RabbitMQ.Core
{
    public class RabbitMqMessageConsumerFactory : IRabbitMqMessageConsumerFactory, IDisposable
    {
        protected RabbitMqMessageConsumer Consumer;

        public RabbitMqMessageConsumerFactory(RabbitMqMessageConsumer consumer)
        {
            Consumer = consumer;
        }

        public IRabbitMqMessageConsumer Create(
            ExchangeDeclareConfiguration exchange,
            QueueDeclareConfiguration queue,
            string connectionName = null)
        {
            Consumer.Initialize(exchange, queue, connectionName);
            return Consumer;
        }

        public void Dispose()
        {
            Consumer?.Dispose();
        }
    }
}
