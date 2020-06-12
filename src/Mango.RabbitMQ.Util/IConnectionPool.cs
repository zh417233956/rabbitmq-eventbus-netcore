using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public interface IConnectionPool : IDisposable
    {
        IConnection Get(string connectionName = null);
    }
}
