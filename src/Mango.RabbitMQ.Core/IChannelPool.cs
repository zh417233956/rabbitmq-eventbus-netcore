using System;

namespace Mango.RabbitMQ.Core
{
    public interface IChannelPool : IDisposable
    {
        IChannelAccessor Acquire(string channelName = null, string connectionName = null);
    }
}
