using Mango.RabbitMQ.Core;
using Mango.RabbitMQ.EventBus.RabbitMQ;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTestRMQEventBus
{
    public class UnitTest1
    {
        private readonly ILogger<UnitTest1> _logger;
        private IRabbitMQEventBus _rabbitmqEvent;
        private RabbitBasicDemo _rabbitBasicDemo;
        public UnitTest1()
        {
            var server = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                );
            var serviceProvider = server.Services;
            _rabbitmqEvent = serviceProvider.GetService<IRabbitMQEventBus>();
            _rabbitBasicDemo = serviceProvider.GetService<RabbitBasicDemo>();
            _logger = serviceProvider.GetService<ILogger<UnitTest1>>();
        }
        [Fact]
        public void Publish()
        {
            _rabbitmqEvent.Subscribe<string>((msg) =>
            {
                _logger.LogInformation($"subscribe:{msg}");
                return Task.FromResult<object>(null);
            });

            _rabbitmqEvent.PublishAsync(typeof(string), "hello rmq eventbus~!");

            Console.WriteLine("published");
        }
        [Fact]
        public void Basic_pub()
        {
            _rabbitBasicDemo.Subscribe();
            _rabbitBasicDemo.Publish("hello basic publish");
            _rabbitBasicDemo.Publish("hello basic publish 2");
        }
    }
}
