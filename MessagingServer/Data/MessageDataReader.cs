using Microsoft.Data.Sqlite;
using MySql.Data.MySqlClient;
using System;
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
            return GetDataReader(Query);
        }

        public static IDataReader GetMessagesForUser(String User)
        {
            String Query = "SELECT * FROM messages_server WHERE MessageUser='" + User + "' ORDER BY SentTime DESC";
            return GetDataReader(Query);
        }

        public static IDataReader GetGroups()
        {
            String Query = "SELECT DISTINCT GroupName FROM group_members";
            return GetDataReader(Query);
        }

        public static IDataReader GetGroupMembers(String Group)
        {
            String Query = "SELECT DISTINCT MemberName FROM group_members WHERE GroupName='" + Group + "'";
            return GetDataReader(Query);
        }

        public static IDataReader GetUserMessages(String MessageFrom, String MessageTo)
        {
            String Query = "SELECT  * FROM messages_user a WHERE (a.MessageFrom = '" + MessageFrom + "' AND a.MessageTo = '" + MessageTo + "') OR (a.MessageFrom = '" + MessageTo + "' AND a.MessageTo = '" + MessageFrom + "') ORDER BY SentTime";
            return GetDataReader(Query);
        }

        public static IDataReader GetMessagedUsers(String User)
        {
            String Query = "SELECT DISTINCT UserName FROM (SELECT MessageTo AS 'UserName' FROM messages_user WHERE MessageFrom='" + User + "' UNION ALL SELECT MessageFrom AS 'UserName' FROM messages_user WHERE MessageTo='" + User + "') as M";
            return GetDataReader(Query);
        }

        // Return a IDataReader depending on whether testing in memory database is used or online in production
        private static IDataReader GetDataReader(string Query)
        {
            if (IsTesting)
            {
                SqliteConnection Connection = SqliteConnection();
                SqliteCommand Command = new SqliteCommand(Query, Connection);
                Connection.Open();

                SqliteDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
            else
            {
                MySqlConnection Connection = SqlConnection();
                MySqlCommand Command = new MySqlCommand(Query, Connection);
                Connection.Open();

                MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

                return reader;
            }
        }

        // Execute a SQL query with the given parameters
        public static void ExecuteCommand(String Query, Dictionary<string, object> Parameters)
        {
            if (IsTesting)
            {
                CreateEmptyTablesForTests();

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

        // Create empty tables in the in memory sqlite for testing. These commands are different than production MySQL
        private static void CreateEmptyTablesForTests()
        {
            string Query1 = "CREATE TABLE IF NOT EXISTS messages_server(Id INTEGER PRIMARY KEY, SentTime datetime, MessageRead BIT, Content varchar(8000), MessageCategory varchar(255), MessageUser varchar(255))";
            string Query2 = "CREATE TABLE IF NOT EXISTS messages_user(Id INTEGER PRIMARY KEY, SentTime datetime, Content varchar(8000), MessageTo varchar(255), MessageFrom varchar(255))";
            string Query3 = "CREATE TABLE IF NOT EXISTS group_members(Id INTEGER PRIMARY KEY, GroupName varchar(255), MemberName varchar(255))";

            SqliteConnection Connection = SqliteConnection();
            Connection.Open();

            SqliteCommand Command = new SqliteCommand(Query1, Connection);
            Command.ExecuteNonQuery();

            SqliteCommand Command2 = new SqliteCommand(Query2, Connection);
            Command2.ExecuteNonQuery();

            SqliteCommand Command3 = new SqliteCommand(Query3, Connection);
            Command3.ExecuteNonQuery();
        }

        // Object for MySqlConnection
        private static MySqlConnection SqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        // ONLY FOR TESTING. Object for SqliteConnection.
        private static SqliteConnection sqliteConnection = null;

        private static SqliteConnection SqliteConnection()
        {
            if (sqliteConnection == null)
                sqliteConnection = new SqliteConnection("Filename=:memory:");

            return sqliteConnection;
        }
    }
}
