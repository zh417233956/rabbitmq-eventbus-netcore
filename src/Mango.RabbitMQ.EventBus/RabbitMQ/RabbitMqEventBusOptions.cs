using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus.RabbitMQ
{
    public class RabbitMqEventBusOptions
    {
        public string ConnectionName { get; set; }

        public string ClientName { get; set; }

        public string ExchangeName { get; set; }
    }
}
