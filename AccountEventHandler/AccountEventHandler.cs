using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountEventHandler(IAccountService accountService) : IAccountEventHandler
    {
        public IAccountService AccountService { get; set; } = accountService;

        public void HandleAccountCreatedEvent(AccountEvent accountCreatedEvent)
        {
            Account account = new Account(accountCreatedEvent.Id, accountCreatedEvent.Name, CurrencyCode.DKK, AccountStatus.Active);
            AccountService.AddAccount(account);
        }

        public void HandleAccountUpdatedEvent(AccountEvent accountUpdatedEvent)
        {
            Account account = new Account(accountUpdatedEvent.Id, accountUpdatedEvent.Name, CurrencyCode.DKK, AccountStatus.Active);
            AccountService.UpdateAccount(account);
        }

        public void HandleAccountSettledEvent(AccountEvent accountSettledEvent)
        {
            AccountService.SettleAccount(accountSettledEvent.Id);
        }


    }
}
