using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public static class RabbitMqConsts
    {
        public static class DeliveryModes
        {
            public const int NonPersistent = 1;

            public const int Persistent = 2;
        }
    }
}
