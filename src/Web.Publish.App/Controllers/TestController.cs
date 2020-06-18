using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.RabbitMQ.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Publish.App.Module;

namespace Web.Publish.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;
        private IRabbitMQEventBus _rabbitmqEvent;

        public TestController(ILogger<TestController> logger, IRabbitMQEventBus rabbitmqEvent)
        {
            _logger = logger;
            _rabbitmqEvent = rabbitmqEvent;
        }
        [HttpGet("Index")]
        public string Index()
        {
            //_rabbitmqEvent.Subscribe<MySimpleEventData, MySimpleDistributedTransientEventHandler>();
            _rabbitmqEvent.Subscribe<MySimpleEventData>(new MySimpleDistributedTransientEventHandler());

            _rabbitmqEvent.Subscribe<MySimpleEventData>((a) =>
            {
                //throw new NotImplementedException();
                return Task.FromResult<object>(null);
            });

            return "Hello World!";
        }

        private Task SubFun(MySimpleEventData arg)
        {
            return Task.FromResult<object>(null);
            //throw new NotImplementedException();
        }

        [HttpGet("Get")]
        public async Task<string> GetAsync()
        {
            //_logger.LogInformation("Hello World!");

            await _rabbitmqEvent.PublishAsync(new MySimpleEventData(1));

            return "Hello RabbitMQ!";
        }
    }
    public class MySimpleEventData
    {
        public int Value { get; set; }

        public MySimpleEventData(int value)
        {
            Value = value;
        }
    }
    public class MySimpleDistributedTransientEventHandler : Mango.RabbitMQ.EventBus.IMQEventHandler<MySimpleEventData>, IDisposable
    {
        public static int HandleCount { get; set; }

        public static int DisposeCount { get; set; }

        public Task HandleEventAsync(MySimpleEventData eventData)
        {
            ++HandleCount;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            ++DisposeCount;
        }
    }
}