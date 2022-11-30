using MySql.Data.MySqlClient;
using System.Data;

namespace MessagingServer.Data
{
    // A class for getting reader for select queries or to execute sql queries
    public class MessageDataReader
    {
        // The Environment variable for the connection string,
        // It contains the server, user, password, database name to connect to sql server.
        private static string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        public static IDataReader GetMessages()
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server ORDER BY SentTime DESC", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagesForUser(String User)
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server WHERE MessageUser='" + User + "' ORDER BY SentTime DESC", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroups()
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT GroupName FROM group_members", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroupMembers(String Group)
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT MemberName FROM group_members WHERE GroupName='" + Group + "'", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetUserMessages(String MessageFrom, String MessageTo)
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT  * FROM messages_user a WHERE (a.MessageFrom = '" + MessageFrom + "' AND a.MessageTo = '" + MessageTo + "') OR (a.MessageFrom = '" + MessageTo + "' AND a.MessageTo = '" + MessageFrom + "') ORDER BY SentTime", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagedUsers(String User)
        {
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT DISTINCT UserName FROM (SELECT MessageTo AS 'UserName' FROM messages_user WHERE MessageFrom='" + User + "' UNION ALL SELECT MessageFrom AS 'UserName' FROM messages_user WHERE MessageTo='" + User + "') as M", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
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

        // Object for MySqlConnection
        public static MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

    }
}
