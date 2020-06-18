using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus
{
    public interface IEventNameProvider
    {
        string GetName(Type eventType);
    }
}
