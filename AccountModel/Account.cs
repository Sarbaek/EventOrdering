namespace EventOrdering.DomainModel
{
    public class Account(int id, string accountName, CurrencyCode currencyCode, AccountStatus accountStatus)
    {

        public int Id { get; set; } = id;
        public string AccountName { get; set; } = accountName;
        public CurrencyCode CurrencyCode { get; set; } = currencyCode;
        public AccountStatus AccountStatus { get; set; } = accountStatus;
    }
}
