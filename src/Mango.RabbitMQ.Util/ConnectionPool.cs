using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public class ConnectionPool : IConnectionPool
    {
        protected RabbitMqOptions Options { get; }

        protected ConcurrentDictionary<string, IConnection> Connections { get; }

        private bool _isDisposed;

        //public ConnectionPool(IOptions<RabbitMqOptions> options)
        public ConnectionPool(RabbitMqOptions options)
        {
            Options = options;
            Connections = new ConcurrentDictionary<string, IConnection>();
        }

        public virtual IConnection Get(string connectionName = null)
        {
            connectionName = connectionName
                             ?? RabbitMqConnections.DefaultConnectionName;
            var conn = Options
                    .Connections
                    .GetOrDefault(connectionName)
                    .CreateConnection();
            return Connections.GetOrAdd(connectionName, conn);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (var connection in Connections.Values)
            {
                try
                {
                    connection.Dispose();
                }
                catch
                {

                }
            }

            Connections.Clear();
        }
    }
}
