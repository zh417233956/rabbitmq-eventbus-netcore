using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Mango.RabbitMQ.Util
{
    [Serializable]
    public class RabbitMqConnections : Dictionary<string, ConnectionFactory>
    {
        public const string DefaultConnectionName = "Default";

        public ConnectionFactory Default
        {
            get => this[DefaultConnectionName];
            set => this[DefaultConnectionName] = value;
        }

        public RabbitMqConnections()
        {
            Default = new ConnectionFactory();
        }

        public ConnectionFactory GetOrDefault(string connectionName)
        {
            if (TryGetValue(connectionName, out var connectionFactory))
            {
                return connectionFactory;
            }

            return Default;
        }
    }
}
