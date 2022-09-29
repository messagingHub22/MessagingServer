using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MessagingServer.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageDataController : ControllerBase
    {

        private readonly ILogger<MessageDataController> _logger;

        private string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public MessageDataController(ILogger<MessageDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet("getMessages")]
        public IEnumerable<MessageData> GetMessages()
        {
            var messages = new List<MessageData>();

            MySqlConnection connection = SqlConnection();
            MySqlCommand command = new MySqlCommand("SELECT * FROM messages_server", connection);
            connection.Open();

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(new MessageData()
                    {
                        Id = reader.GetInt32("Id"),
                        SentTime = reader.GetDateTime("SentTime"),
                        MessageRead = reader.GetInt16("MessageRead") == 1,
                        Content = reader.GetString("Content"),
                        MessageCategory = reader.GetString("MessageCategory"),
                        MessageUser = reader.GetString("MessageUser")
                    });
                }
            }

            return messages;
        }

        private MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}