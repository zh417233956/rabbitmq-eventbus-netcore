using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus.RabbitMQ
{
    public interface IRabbitMQEventBus : IEventBus
    {
        /// <summary>
        /// Registers to an event. 
        /// Same (given) instance of the handler is used for all event occurrences.
        /// </summary>
        /// <typeparam name="TEvent">Event type</typeparam>
        /// <param name="handler">Object to handle the event</param>
        IDisposable Subscribe<TEvent>(IMQEventHandler<TEvent> handler)
            where TEvent : class;
    }
}
