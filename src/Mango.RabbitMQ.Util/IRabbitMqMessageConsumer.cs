using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Mango.RabbitMQ.Util
{
    public interface IRabbitMqMessageConsumer
    {
        Task BindAsync(string routingKey);

        Task UnbindAsync(string routingKey);

        void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback);
    }
}
