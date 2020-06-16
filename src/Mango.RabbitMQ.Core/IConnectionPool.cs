using RabbitMQ.Client;
using System;

namespace Mango.RabbitMQ.Core
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}
