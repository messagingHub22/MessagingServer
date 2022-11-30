using MessagingServer.Data;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace MessagingServer.Controllers
{
    [ApiController]
    [Route("api")]
    public class MessageDataController : ControllerBase
    {

        private readonly ILogger<MessageDataController> _logger;

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
                    Id = Int32.Parse(Reader["Id"].ToString()),
                    SentTime = DateTime.Parse(Reader["SentTime"].ToString()),
                    MessageRead = Int32.Parse(Reader["MessageRead"].ToString()) == 1,
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

            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("@SentTime", SentTime);
            Parameters.Add("@MessageRead", false);
            Parameters.Add("@Content", Content);
            Parameters.Add("@MessageCategory", MessageCategory);
            Parameters.Add("@MessageUser", MessageUser);

            MessageDataReader.ExecuteCommand(Query, Parameters);
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
                    Id = Int32.Parse(Reader["Id"].ToString()),
                    SentTime = DateTime.Parse(Reader["SentTime"].ToString()),
                    MessageRead = Int32.Parse(Reader["MessageRead"].ToString()) == 1,
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

            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("@Id", Id);

            MessageDataReader.ExecuteCommand(Query, Parameters);
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

            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("@GroupName", GroupName);
            Parameters.Add("@MemberName", MemberName);

            MessageDataReader.ExecuteCommand(Query, Parameters);
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

            Dictionary<string, object> Parameters = new Dictionary<string, object>();
            Parameters.Add("@SentTime", SentTime);
            Parameters.Add("@Content", Content);
            Parameters.Add("@MessageFrom", MessageFrom);
            Parameters.Add("@MessageTo", MessageTo);

            MessageDataReader.ExecuteCommand(Query, Parameters);
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
                    Id = Int32.Parse(Reader["Id"].ToString()),
                    SentTime = DateTime.Parse(Reader["SentTime"].ToString()),
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

        // Method used for testing. Do not call from web. HttpPost added to remove error
        [HttpPost("doNotCallThis")]
        public void SetCustomReader(IDataReader reader)
        {
            TestReader = reader;
        }

    }
}
