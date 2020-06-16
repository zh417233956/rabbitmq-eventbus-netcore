﻿using RabbitMQ.Client;
using System.Collections.Generic;

namespace Mango.RabbitMQ.Core
{
    public class QueueDeclareConfiguration
    {
        public string QueueName { get; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Arguments { get; }

        public QueueDeclareConfiguration(
            string queueName,
            bool durable = true,
            bool exclusive = false,
            bool autoDelete = false)
        {
            QueueName = queueName;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = new Dictionary<string, object>();
        }

        public virtual QueueDeclareOk Declare(IModel channel)
        {
            return channel.QueueDeclare(
                queue: QueueName,
                durable: Durable,
                exclusive: Exclusive,
                autoDelete: AutoDelete,
                arguments: Arguments
            );
        }
    }
}
