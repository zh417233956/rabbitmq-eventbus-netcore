using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace Mango.RabbitMQ.Core
{
    public class ConnectionPool : IConnectionPool
    {
        protected RabbitMqOptions Options { get; }

        protected ConcurrentDictionary<string, IConnection> Connections { get; }

        private bool _isDisposed;

        public ConnectionPool(IOptions<RabbitMqOptions> options)
        {
            Options = options.Value;
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
