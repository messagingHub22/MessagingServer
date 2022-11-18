namespace MessagingServer.Data
{
    public class MessageUser
    {
        // Time of sending the message
        public DateTime SentTime { get; set; }

        // Unique id of message
        public int Id { get; set; }

        // The content of message
        public string? Content { get; set; }

        // The user from which message is sent
        public string? MessageFrom { get; set; }

        // The user to which message is sent to
        public string? MessageTo { get; set; }
    }
}
