﻿using Newtonsoft.Json;
using System;
using System.Text;

namespace Mango.RabbitMQ.Core
{
    public class Utf8JsonRabbitMqSerializer : IRabbitMqSerializer
    {
        public byte[] Serialize(object obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public object Deserialize(byte[] value, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value), type);
        }
    }
}
