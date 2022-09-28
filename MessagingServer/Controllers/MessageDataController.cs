using Microsoft.AspNetCore.Mvc;

namespace MessagingServer.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageDataController : ControllerBase
    {

        private readonly ILogger<MessageDataController> _logger;

        public MessageDataController(ILogger<MessageDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getMessages")]
        public IEnumerable<MessageData> GetMessages()
        {
            return Enumerable.Range(1, 5).Select(index => new MessageData
            {
                SentTime = DateTime.Now.AddDays(index),
                Id = Random.Shared.Next(-20, 55),
                MessageRead = false,
                Content = String.Concat("Message ", index),
                MessageCategory = "Other",
                MessageUser = "Person1"
            })
            .ToArray();
        }


    }
}