namespace EventOrdering
{
    public class AccountEvent(DateTimeOffset eventReceived, DateTimeOffset eventProcessed, int id, string currency, string name)
    {
        public DateTimeOffset EventReceived { get; set; } = eventReceived;
        public DateTimeOffset EventProcessed { get; set; } = eventProcessed;
        public int Id { get; set; } = id;
        public string Currency { get; set; } = currency;
        public string Name { get; set; } = name;
    }
}
