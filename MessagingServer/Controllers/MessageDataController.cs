using MessagingServer.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

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
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server ORDER BY SentTime DESC", Connection);
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
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server WHERE MessageUser='" + User + "' ORDER BY SentTime DESC", Connection);
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

        // Mark the message with the given Id as read
        [HttpPost("markMessageRead")]
        public void MarkMessageRead(string Id)
        {
            MySqlConnection Connection = SqlConnection();
            Connection.Open();

            String Query = "UPDATE messages_server SET MessageRead = 1 WHERE Id = @Id";

            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@Id", Id);

            Command.ExecuteNonQuery();

            Connection.Close();
        }

        // Get all the groups
        [HttpGet("getGroups")]
        public IEnumerable<String> GetGroups()
        {
            var Groups = new List<String>();

            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT GroupName FROM group_members", Connection);
            Connection.Open();

            using (MySqlDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Groups.Add(reader.GetString("GroupName"));
                }
            }

            Connection.Close();

            return Groups;
        }

        // Add member to a group
        [HttpPost("addMemberToGroup")]
        public void AddMemberToGroup(String GroupName, String MemberName)
        {
            MySqlConnection Connection = SqlConnection();
            Connection.Open();

            String Query = "INSERT INTO group_members (GroupName, MemberName) VALUES (@GroupName, @MemberName)";

            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@GroupName", GroupName);
            Command.Parameters.AddWithValue("@MemberName", MemberName);

            Command.ExecuteNonQuery();

            Connection.Close();
        }

        // Get members from a group
        [HttpGet("getGroupMembers")]
        public IEnumerable<String> GetGroupMembers(String Group)
        {
            var GroupMembers = new List<String>();

            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT MemberName FROM group_members WHERE GroupName='" + Group + "'", Connection);
            Connection.Open();

            using (MySqlDataReader reader = Command.ExecuteReader())
            {
                while (reader.Read())
                {
                    GroupMembers.Add(reader.GetString("MemberName"));
                }
            }

            Connection.Close();

            return GroupMembers;
        }

        // Send a message to all users in a group from server
        [HttpPost("sendMessageToGroup")]
        public void SendMessageToGroup(String SentTime, String Content, String MessageCategory, String MessageGroup)
        {
            List<string> GroupMembers = (List<string>) GetGroupMembers(MessageGroup);

            foreach (var Member in GroupMembers)
            {
                SendMessage(SentTime, Content, MessageCategory, Member);
            }
        }

        // Object for MySqlConnection
        private MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}