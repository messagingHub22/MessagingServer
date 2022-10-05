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

        // The Environment variable for the connection string,
        // It contains the server, user, password, database name to connect to sql server.
        private string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public MessageDataController(ILogger<MessageDataController> logger)
        {
            _logger = logger;
        }

        // Get all the messages sent to all users from server
        [HttpGet("getMessages")]
        public IEnumerable<MessageData> GetMessages()
        {
            var Messages = new List<MessageData>();

            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server", Connection);
            Connection.Open();

            using (MySqlDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Messages.Add(new MessageData()
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

            Connection.Close();

            return Messages;
        }

        // Send a message to a user from server
        [HttpPost("sendMessage")]
        public void SendMessage(String SentTime, String Content, String MessageCategory, String MessageUser)
        {
            MySqlConnection Connection = SqlConnection();
            Connection.Open();

            String Query = "INSERT INTO messages_server (SentTime, MessageRead, Content, MessageCategory, MessageUser) VALUES (@SentTime, @MessageRead, @Content, @MessageCategory, @MessageUser)";

            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@SentTime", SentTime);
            Command.Parameters.AddWithValue("@MessageRead", 0);
            Command.Parameters.AddWithValue("@Content", Content);
            Command.Parameters.AddWithValue("@MessageCategory", MessageCategory);
            Command.Parameters.AddWithValue("@MessageUser", MessageUser);

            Command.ExecuteNonQuery();

            Connection.Close();
        }

        // Get the messages sent to a user from server
        [HttpGet("getMessagesForUser")]
        public IEnumerable<MessageData> GetMessagesForUser(String User)
        {
            var Messages = new List<MessageData>();

            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server WHERE MessageUser='" + User + "'", Connection);
            Connection.Open();

            using (MySqlDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Messages.Add(new MessageData()
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

            Connection.Close();

            return Messages;
        }

        // Object for MySqlConnection
        private MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}