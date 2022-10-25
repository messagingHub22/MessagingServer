namespace MessagingServer.Data
{
    public class GroupData
    {
        // Unique id of the group member
        public int Id { get; set; }

        // Name of the group
        public string? GroupName { get; set; }

        // The user which is a member of this group
        public string? MemberName { get; set; }
    }
}
