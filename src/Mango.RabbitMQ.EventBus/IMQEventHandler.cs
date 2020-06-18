using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mango.RabbitMQ.EventBus
{
    public interface IMQEventHandler<in TEvent> : IEventHandler
    {
        /// <summary>
        /// Handler handles the event by implementing this method.
        /// </summary>
        /// <param name="eventData">Event data</param>
        Task HandleEventAsync(TEvent eventData);
    }
}
