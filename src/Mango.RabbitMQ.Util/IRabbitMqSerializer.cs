using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public interface IRabbitMqSerializer
    {
        byte[] Serialize(object obj);

        object Deserialize(byte[] value, Type type);
    }
}
