using Google.Protobuf;
using MessagingServer.Controllers;
using MySql.Data.MySqlClient;
using System.Data;

namespace MessagingServer.Data
{
    // A class for getting reader for select queries
    public class MessageDataReader
    {

        public static IDataReader GetMessages() {
            MySqlConnection Connection = MessageDataController.SqlConnection();
            MySqlCommand Command = new MySqlCommand("SELECT * FROM messages_server ORDER BY SentTime DESC", Connection);
            Connection.Open();

            MySqlDataReader reader = Command.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }


    }
}
