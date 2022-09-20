using Microsoft.AspNetCore.Mvc;

namespace MessagingServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageDataController : ControllerBase
    {

        private readonly ILogger<MessageDataController> _logger;

        public MessageDataController(ILogger<MessageDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetMessageData")]
        public IEnumerable<MessageData> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new MessageData
            {
                Date = DateTime.Now.AddDays(index),
                Id = Random.Shared.Next(-20, 55),
                Read = false,
                Content = String.Concat("Message", index),
                Flag = "Other"
            })
            .ToArray();
        }
    }
}