using RabbitMQ.Client;
using System;

namespace Mango.RabbitMQ.Util
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}
