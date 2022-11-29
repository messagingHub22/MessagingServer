using Google.Protobuf;
using MessagingServer.Controllers;
using MySql.Data.MySqlClient;
using System.Data;

namespace MessagingServer.Data
{
    // A class for getting reader for select queries
    public class MessageDataReader
    {

        public static IDataReader GetMessages()
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server ORDER BY SentTime DESC", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagesForUser(String User)
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server WHERE MessageUser='" + User + "' ORDER BY SentTime DESC", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroups()
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT GroupName FROM group_members", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroupMembers(String Group)
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT MemberName FROM group_members WHERE GroupName='" + Group + "'", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetUserMessages(String MessageFrom, String MessageTo)
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT  * FROM messages_user a WHERE (a.MessageFrom = '" + MessageFrom + "' AND a.MessageTo = '" + MessageTo + "') OR (a.MessageFrom = '" + MessageTo + "' AND a.MessageTo = '" + MessageFrom + "') ORDER BY SentTime", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagedUsers(String User)
        {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT UserName FROM (SELECT MessageTo AS 'UserName' FROM messages_user WHERE MessageFrom='" + User + "' UNION ALL SELECT MessageFrom AS 'UserName' FROM messages_user WHERE MessageTo='" + User + "') as M", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

    }
}
