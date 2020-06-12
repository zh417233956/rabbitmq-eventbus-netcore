using System;

namespace Mango.RabbitMQ.Util
{
    public class RabbitMqOptions
    {
        public RabbitMqConnections Connections { get; }

        public RabbitMqOptions()
        {
            Connections = new RabbitMqConnections();
        }
    }
}
