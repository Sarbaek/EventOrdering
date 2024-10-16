namespace EventOrdering.DomainModel
{
    public class Account
    {

        public int Id { get; set; }
        public string? AccountName { get; set; }
        public CurrencyCode CurrencyCode { get; set; }
        public AccountStatus AccountStatus { get; set; }
        public DateTimeOffset LatestChange { get; set; }
    }
}
