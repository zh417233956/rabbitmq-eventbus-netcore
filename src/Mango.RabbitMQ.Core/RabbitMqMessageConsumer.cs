﻿using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Mango.RabbitMQ.Core
{
    public class RabbitMqMessageConsumer : IRabbitMqMessageConsumer, IDisposable
    {
        public ILogger<RabbitMqMessageConsumer> Logger { get; set; }
        protected MgoTimer Timer { get; }

        protected IConnectionPool ConnectionPool { get; }

        protected ExchangeDeclareConfiguration Exchange { get; private set; }

        protected QueueDeclareConfiguration Queue { get; private set; }

        protected string ConnectionName { get; private set; }

        protected ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>> Callbacks { get; }

        protected IModel Channel { get; private set; }

        protected ConcurrentQueue<QueueBindCommand> QueueBindCommands { get; }

        protected object ChannelSendSyncLock { get; } = new object();

        public RabbitMqMessageConsumer(IConnectionPool connectionPool, ILogger<RabbitMqMessageConsumer> logger)
        {
            Logger = logger;
            ConnectionPool = connectionPool;
            QueueBindCommands = new ConcurrentQueue<QueueBindCommand>();
            Callbacks = new ConcurrentBag<Func<IModel, BasicDeliverEventArgs, Task>>();
            Timer = new MgoTimer();
            Timer.Period = 5000; //5 sec.
            Timer.Elapsed += Timer_Elapsed;
            Timer.RunOnStart = true;
        }

        public void Initialize(
            ExchangeDeclareConfiguration exchange,
            QueueDeclareConfiguration queue,
            string connectionName = null)
        {
            Exchange = exchange;
            Queue = queue;
            ConnectionName = connectionName;
            Timer.Start();
        }

        public virtual async Task BindAsync(string routingKey)
        {
            QueueBindCommands.Enqueue(new QueueBindCommand(QueueBindType.Bind, routingKey));
            await TrySendQueueBindCommandsAsync();
        }

        public virtual async Task UnbindAsync(string routingKey)
        {
            QueueBindCommands.Enqueue(new QueueBindCommand(QueueBindType.Unbind, routingKey));
            await TrySendQueueBindCommandsAsync();
        }

        protected virtual void TrySendQueueBindCommands()
        {
            try
            {
                while (!QueueBindCommands.IsEmpty)
                {
                    if (Channel == null || Channel.IsClosed)
                    {
                        return;
                    }

                    lock (ChannelSendSyncLock)
                    {
                        QueueBindCommands.TryPeek(out var command);

                        switch (command.Type)
                        {
                            case QueueBindType.Bind:
                                Channel.QueueBind(
                                    queue: Queue.QueueName,
                                    exchange: Exchange.ExchangeName,
                                    routingKey: command.RoutingKey
                                );
                                break;
                            case QueueBindType.Unbind:
                                Channel.QueueUnbind(
                                    queue: Queue.QueueName,
                                    exchange: Exchange.ExchangeName,
                                    routingKey: command.RoutingKey
                                );
                                break;
                            default:
                                throw new Exception($"Unknown {nameof(QueueBindType)}: {command.Type}");
                        }

                        QueueBindCommands.TryDequeue(out command);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
            }
        }

        protected virtual Task TrySendQueueBindCommandsAsync()
        {
            TrySendQueueBindCommands();
            return Task.CompletedTask;
        }

        public virtual void OnMessageReceived(Func<IModel, BasicDeliverEventArgs, Task> callback)
        {
            Callbacks.Add(callback);
        }

        protected virtual void Timer_Elapsed(object sender, EventArgs e)
        {
            if (Channel == null || Channel.IsOpen == false)
            {
                TryCreateChannel();
                TrySendQueueBindCommands();
            }
        }

        protected virtual void TryCreateChannel()
        {
            DisposeChannel();

            try
            {
                var channel = ConnectionPool
                    .Get(ConnectionName)
                    .CreateModel();

                channel.ExchangeDeclare(
                    exchange: Exchange.ExchangeName,
                    type: Exchange.Type,
                    durable: Exchange.Durable,
                    autoDelete: Exchange.AutoDelete,
                    arguments: Exchange.Arguments
                );

                channel.QueueDeclare(
                    queue: Queue.QueueName,
                    durable: Queue.Durable,
                    exclusive: Queue.Exclusive,
                    autoDelete: Queue.AutoDelete,
                    arguments: Queue.Arguments
                );

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, basicDeliverEventArgs) =>
                {
                    await HandleIncomingMessage(channel, basicDeliverEventArgs);
                };

                channel.BasicConsume(
                    queue: Queue.QueueName,
                    autoAck: false,
                    consumer: consumer
                );

                Channel = channel;
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
            }
        }

        protected virtual async Task HandleIncomingMessage(IModel channel, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            try
            {
                foreach (var callback in Callbacks)
                {
                    await callback(channel, basicDeliverEventArgs);
                }

                channel.BasicAck(basicDeliverEventArgs.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
                //channel.BasicReject(basicDeliverEventArgs.DeliveryTag,true);
                channel.BasicNack(basicDeliverEventArgs.DeliveryTag, false, true);
            }
        }

        protected virtual void DisposeChannel()
        {
            if (Channel == null)
            {
                return;
            }

            try
            {
                Channel.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.Message);
            }
        }

        public virtual void Dispose()
        {
            Timer.Stop();
            DisposeChannel();
        }

        protected class QueueBindCommand
        {
            public QueueBindType Type { get; }

            public string RoutingKey { get; }

            public QueueBindCommand(QueueBindType type, string routingKey)
            {
                Type = type;
                RoutingKey = routingKey;
            }
        }

        protected enum QueueBindType
        {
            Bind,
            Unbind
        }
    }
}
