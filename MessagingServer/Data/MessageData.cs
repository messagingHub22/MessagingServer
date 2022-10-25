namespace MessagingServer.Data
{
    public class MessageData
    {
        // Time of sending the message
        public DateTime SentTime { get; set; }

        // Unique id of message
        public int Id { get; set; }

        // Whether message is read
        public bool MessageRead { get; set; }

        // The content of message
        public string? Content { get; set; }

        // Category of message is (Exports, SystemEvents, WorkTasks, Other etc.)
        public string? MessageCategory { get; set; }

        // The user to which message is sent
        public string? MessageUser { get; set; }
    }
}