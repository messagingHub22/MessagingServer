namespace MessagingServer
{
    public class MessageData
    {
        public DateTime Date { get; set; }

        public int Id { get; set; }

        public bool Read { get; set; }

        public string? Content { get; set; }

        public string? Flag { get; set; }
    }
}