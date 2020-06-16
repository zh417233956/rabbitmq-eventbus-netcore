namespace Mango.RabbitMQ.Core
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
