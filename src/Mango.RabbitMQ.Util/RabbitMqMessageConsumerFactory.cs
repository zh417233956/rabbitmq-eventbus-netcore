using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public class RabbitMqMessageConsumerFactory : IRabbitMqMessageConsumerFactory, IDisposable
    {
        //protected IServiceScope ServiceScope { get; }

        //public RabbitMqMessageConsumerFactory(IServiceScopeFactory serviceScopeFactory)
        //{
        //    ServiceScope = serviceScopeFactory.CreateScope();
        //}

        public IRabbitMqMessageConsumer Create(
            ExchangeDeclareConfiguration exchange,
            QueueDeclareConfiguration queue,
            string connectionName = null)
        {
            //var consumer = ServiceScope.ServiceProvider.GetRequiredService<RabbitMqMessageConsumer>();
            //consumer.Initialize(exchange, queue, connectionName);
            //return consumer;

            return null;
        }

        public void Dispose()
        {
            //ServiceScope?.Dispose();
        }
    }
}
