using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private RabbitMqEventBus _rabbitmqEvent;

        public TestController(ILogger<TestController> logger, RabbitMqEventBus rabbitmqEvent)
        {
            _logger = logger;
            _rabbitmqEvent = rabbitmqEvent;
        }

        [HttpGet("Get")]
        public string Get()
        {
            //_logger.LogInformation("Hello World!");

            _rabbitmqEvent.PublishAsync("mis2020_message", "hello rabbitmq routing");

            return "Hello World!";
        }
        [HttpGet("Bind")]
        public async Task<string> BindAsync()
        {
            await _rabbitmqEvent.BindAsync("mis2020_message");
            return "Bind!";
        }
        [HttpGet("UNBind")]
        public async Task<string> UNBindAsync()
        {
            await _rabbitmqEvent.UnbindAsync("mis2020_message");
            return "UNBind!";
        }
    }
}