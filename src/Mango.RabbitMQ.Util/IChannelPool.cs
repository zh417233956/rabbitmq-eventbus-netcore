using System;
using System.Collections.Generic;
using System.Text;

namespace Mango.RabbitMQ.Util
{
    public interface IChannelPool : IDisposable
    {
        IChannelAccessor Acquire(string channelName = null, string connectionName = null);
    }
}
