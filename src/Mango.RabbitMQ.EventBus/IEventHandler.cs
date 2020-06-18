using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.EventBus
{
    public interface IEventHandler
    {

    }

    public interface IEventDataWithInheritableGenericArgument
    {
        /// <summary>
        /// Gets arguments to create this class since a new instance of this class is created.
        /// </summary>
        /// <returns>Constructor arguments</returns>
        object[] GetConstructorArgs();
    }
}
