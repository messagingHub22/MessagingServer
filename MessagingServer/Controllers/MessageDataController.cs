using Google.Protobuf.WellKnownTypes;
using MessagingServer.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MessagingServer.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageDataController : ControllerBase
    {

        private readonly ILogger<MessageDataController> _logger;

        // The Environment variable for the connection string,
        // It contains the server, user, password, database name to connect to sql server.
        private static string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        // To indicate the reader when tests are being run. Used for mock sql reader. Null when tests are not being run
        private IDataReader TestReader = null;

        public MessageDataController(ILogger<MessageDataController> logger)
        {
            _logger = logger;
        }

        // Get all the messages sent to all users from server
        [HttpGet("getMessages")]
        public IEnumerable<MessageData> GetMessages()
        {
            var Messages = new List<MessageData>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetMessages();
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                Messages.Add(new MessageData()
                {
                    Id = (int)((UInt32)Reader["Id"]),
                    SentTime = (DateTime)Reader["SentTime"],
                    MessageRead = (UInt64)Reader["MessageRead"] == 1,
                    Content = (string)Reader["Content"],
                    MessageCategory = (string)Reader["MessageCategory"],
                    MessageUser = (string)Reader["MessageUser"]
                });
            }

            Reader.Close();

            return Messages;
        }

        // Send a message to a user from server
        [HttpPost("sendMessage")]
        public void SendMessage(String SentTime, String Content, String MessageCategory, String MessageUser)
        {
            String Query = "INSERT INTO messages_server (SentTime, MessageRead, Content, MessageCategory, MessageUser) VALUES (@SentTime, @MessageRead, @Content, @MessageCategory, @MessageUser)";

            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            Parameters.Add(new MySqlParameter("@SentTime", SentTime));
            Parameters.Add(new MySqlParameter("@MessageRead", false));
            Parameters.Add(new MySqlParameter("@Content", Content));
            Parameters.Add(new MySqlParameter("@MessageCategory", MessageCategory));
            Parameters.Add(new MySqlParameter("@MessageUser", MessageUser));

            ExecuteCommand(Query, Parameters);
        }

        // Get the messages sent to a user from server
        [HttpGet("getMessagesForUser")]
        public IEnumerable<MessageData> GetMessagesForUser(String User)
        {
            var Messages = new List<MessageData>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetMessagesForUser(User);
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                Messages.Add(new MessageData()
                {
                    Id = (int)((UInt32)Reader["Id"]),
                    SentTime = (DateTime)Reader["SentTime"],
                    MessageRead = (UInt64)Reader["MessageRead"] == 1,
                    Content = (string)Reader["Content"],
                    MessageCategory = (string)Reader["MessageCategory"],
                    MessageUser = (string)Reader["MessageUser"]
                });
            }

            Reader.Close();

            return Messages;
        }

        // Mark the message with the given Id as read
        [HttpPost("markMessageRead")]
        public void MarkMessageRead(string Id)
        {
            String Query = "UPDATE messages_server SET MessageRead = 1 WHERE Id = @Id";

            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            Parameters.Add(new MySqlParameter("@Id", Id));

            ExecuteCommand(Query, Parameters);
        }

        // Get all the groups
        [HttpGet("getGroups")]
        public IEnumerable<String> GetGroups()
        {
            var Groups = new List<String>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetGroups();
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                Groups.Add((string)Reader["GroupName"]);
            }

            Reader.Close();

            return Groups;
        }

        // Add member to a group
        [HttpPost("addMemberToGroup")]
        public void AddMemberToGroup(String GroupName, String MemberName)
        {
            String Query = "INSERT INTO group_members (GroupName, MemberName) VALUES (@GroupName, @MemberName)";

            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            Parameters.Add(new MySqlParameter("@GroupName", GroupName));
            Parameters.Add(new MySqlParameter("@MemberName", MemberName));

            ExecuteCommand(Query, Parameters);
        }

        // Get members from a group
        [HttpGet("getGroupMembers")]
        public IEnumerable<String> GetGroupMembers(String Group)
        {
            var GroupMembers = new List<String>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetGroupMembers(Group);
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                GroupMembers.Add((string)Reader["MemberName"]);
            }

            Reader.Close();

            return GroupMembers;
        }

        // Send a message to all users in a group from server
        [HttpPost("sendMessageToGroup")]
        public void SendMessageToGroup(String SentTime, String Content, String MessageCategory, String MessageGroup)
        {
            List<string> GroupMembers = (List<string>)GetGroupMembers(MessageGroup);

            foreach (var Member in GroupMembers)
            {
                SendMessage(SentTime, Content, MessageCategory, Member);
            }
        }

        // Send a message from a user to other user
        [HttpPost("sendUserMessage")]
        public void SendUserMessage(String SentTime, String Content, String MessageFrom, String MessageTo)
        {
            String Query = "INSERT INTO messages_user (SentTime, Content, MessageFrom, MessageTo) VALUES (@SentTime, @Content, @MessageFrom, @MessageTo)";

            List<MySqlParameter> Parameters = new List<MySqlParameter>();
            Parameters.Add(new MySqlParameter("@SentTime", SentTime));
            Parameters.Add(new MySqlParameter("@Content", Content));
            Parameters.Add(new MySqlParameter("@MessageFrom", MessageFrom));
            Parameters.Add(new MySqlParameter("@MessageTo", MessageTo));

            ExecuteCommand(Query, Parameters);
        }

        // Get all messages between from a user to other user
        [HttpGet("getUserMessages")]
        public IEnumerable<MessageUser> GetUserMessages(String MessageFrom, String MessageTo)
        {
            var Messages = new List<MessageUser>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetUserMessages(MessageFrom, MessageTo);
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                Messages.Add(new MessageUser()
                {
                    Id = (int)((UInt32)Reader["Id"]),
                    SentTime = (DateTime)Reader["SentTime"],
                    Content = (string)Reader["Content"],
                    MessageFrom = (string)Reader["MessageFrom"],
                    MessageTo = (string)Reader["MessageTo"]
                });
            }

            Reader.Close();

            return Messages;
        }

        // Get all the users that a user has messaged or got messages from. 
        [HttpGet("getMessagedUsers")]
        public IEnumerable<String> GetMessagedUsers(String User)
        {
            var Messages = new List<String>();

            IDataReader Reader;
            if (TestReader == null)
            {
                Reader = MessageDataReader.GetMessagedUsers(User);
            }
            else
            {
                Reader = TestReader;
            }

            while (Reader.Read())
            {
                Messages.Add((string)Reader["UserName"]);
            }

            Messages.Sort();

            Reader.Close();

            return Messages;
        }

        // Object for MySqlConnection
        public static MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // Method used for testing. Do not call from web. HttpPost added to remove error
        [HttpPost("doNotCallThis")]
        public void SetCustomReader(IDataReader reader)
        {
            TestReader = reader;
        }

        // Execute a SQL query with the given parameters
        public static void ExecuteCommand(String Query, List<MySqlParameter> Parameters)
        {
            MySqlConnection Connection = SqlConnection();
            Connection.Open();

            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Command.Parameters.AddRange(Parameters.ToArray<MySqlParameter>());

            Command.ExecuteNonQuery();
            Connection.Close();
        }

    }
}
