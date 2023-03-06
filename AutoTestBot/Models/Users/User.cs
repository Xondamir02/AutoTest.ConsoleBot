namespace AutoTestBot.Models.Users
{
    internal class User
    {
        public long ChatId { get; set; }
        public string Name { get; set; }
        public ENextmessage Step { get; set; }
        public Ticket? CurrentTicket { get; set; }

        public List<Ticket> Tickets { get; set; }

        public User()
        {
            Tickets = new List<Ticket>();
        }
    }
}
