using System;

namespace Mango.RabbitMQ.Core
{
    public interface IRabbitMqSerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] value, Type type);
    }
}
