namespace MessagingServer
{
    public class MessageData
    {
        // Time of sending the message
        public DateTime Date { get; set; }

        // Unique id of message
        public int Id { get; set; }

        // Whether message is read
        public bool Read { get; set; }

        // The content of message
        public string? Content { get; set; }

        // Category of message is (Exports, SystemEvents, WorkTasks, Other etc.)
        public string? MessageCategory { get; set; }
    }
}