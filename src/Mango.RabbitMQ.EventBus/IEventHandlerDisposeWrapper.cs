using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus
{
    public interface IEventHandlerDisposeWrapper : IDisposable
    {
        IEventHandler EventHandler { get; }
    }
}
