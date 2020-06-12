using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

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
            //TODO:待完善
            //Default = new ConnectionFactory();
            Default = new ConnectionFactory() { HostName = "192.168.6.88", Port = 5672, UserName = "test", Password = "123456" };
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
