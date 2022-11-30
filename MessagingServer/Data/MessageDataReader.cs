using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data;

namespace MessagingServer.Data
{
    // A class for getting reader for select queries or to execute sql queries
    public class MessageDataReader
    {
        // The Environment variable for the connection string,
        // It contains the server, user, password, database name to connect to sql server.
        private static string ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        // If unit test is running
        public static bool IsTesting = false;

        public static IDataReader GetMessages()
        {
            String Query = "SELECT * FROM messages_server ORDER BY SentTime DESC";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagesForUser(String User)
        {
            String Query = "SELECT * FROM messages_server WHERE MessageUser='" + User + "' ORDER BY SentTime DESC";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroups()
        {
            String Query = "SELECT DISTINCT GroupName FROM group_members";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetGroupMembers(String Group)
        {
            String Query = "SELECT DISTINCT MemberName FROM group_members WHERE GroupName='" + Group + "'";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetUserMessages(String MessageFrom, String MessageTo)
        {
            String Query = "SELECT  * FROM messages_user a WHERE (a.MessageFrom = '" + MessageFrom + "' AND a.MessageTo = '" + MessageTo + "') OR (a.MessageFrom = '" + MessageTo + "' AND a.MessageTo = '" + MessageFrom + "') ORDER BY SentTime";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        public static IDataReader GetMessagedUsers(String User)
        {
            String Query = "SELECT DISTINCT UserName FROM (SELECT MessageTo AS 'UserName' FROM messages_user WHERE MessageFrom='" + User + "' UNION ALL SELECT MessageFrom AS 'UserName' FROM messages_user WHERE MessageTo='" + User + "') as M";
            MySqlConnection Connection = SqlConnection();
            MySqlCommand Command = new MySqlCommand(Query, Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }

        // Execute a SQL query with the given parameters
        public static void ExecuteCommand(String Query, Dictionary<string, object> Parameters)
        {
            if (IsTesting)
            {
                SqliteConnection Connection = SqliteConnection();
                Connection.Open();

                List<SqliteParameter> SqlParameters = new List<SqliteParameter>();
                foreach (var Item in Parameters)
                {
                    SqlParameters.Add(new SqliteParameter(Item.Key, Item.Value));
                }

                SqliteCommand Command = new SqliteCommand(Query, Connection);
                Command.Parameters.AddRange(SqlParameters);

                Command.ExecuteNonQuery();
                Connection.Close();
            }
            else
            {
                MySqlConnection Connection = SqlConnection();
                Connection.Open();

                List<MySqlParameter> SqlParameters = new List<MySqlParameter>();
                foreach (var Item in Parameters)
                {
                    SqlParameters.Add(new MySqlParameter(Item.Key, Item.Value));
                }

                MySqlCommand Command = new MySqlCommand(Query, Connection);
                Command.Parameters.AddRange(SqlParameters.ToArray<MySqlParameter>());

                Command.ExecuteNonQuery();
                Connection.Close();
            }
        }

        // Object for MySqlConnection
        public static MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // ONLY FOR TESTING. Object for SqliteConnection.
        public static SqliteConnection SqliteConnection()
        {
            return new SqliteConnection("Filename=:memory:");
        }
    }
}
