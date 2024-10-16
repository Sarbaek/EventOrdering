using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountEventHandler(IAccountService accountService, ITracingService tracingService) : IAccountEventHandler
    {
        public IAccountService AccountService { get; set; } = accountService;
        public ITracingService TracingService { get; set; } = tracingService;

        public void HandleAccountOpenedEvent(AccountEvent accountOpenedEvent)
        {
            if (!ValidateAccountExists(accountOpenedEvent.Id, out _))
            {
                TracingService.LogError($"Received account opened event but account already exists. Id: {accountOpenedEvent.Id}", this.GetType().Name);
                return;
            }

            Account account = new()
            {
                Id = accountOpenedEvent.Id,
                AccountName = accountOpenedEvent.Name,
                AccountStatus = AccountStatus.Active,
                CurrencyCode = ToCurrencyCode(accountOpenedEvent.Currency),
                LatestChange = accountOpenedEvent.EventReceived,
            };
                
            AccountService.AddAccount(account);

            //TODO: Unpark waiting events
        }

        public void HandleAccountUpdatedEvent(AccountEvent accountUpdatedEvent)
        {
            var existingAccount = AccountService.GetAccount(accountUpdatedEvent.Id);

            // Handle update event arriving before opened event
            if (!ValidateAccountExists(accountUpdatedEvent.Id, out var account))
            {
                TracingService.LogError($"Account with id: {accountUpdatedEvent.Id} does not exist, parking event until account opened event arrives", this.GetType().Name);
                //TODO: Create parkingservice
                return;
            }

            // Handle update event arriving late to the party
            if(account?.LatestChange > accountUpdatedEvent.EventReceived)
            {
                TracingService.LogError($"Account with id: {accountUpdatedEvent.Id} already updated by later event, no more work needed", this.GetType().Name);
                return;
            }

            // All is good, update account
            Account newAccount = new()
            {
                Id = accountUpdatedEvent.Id,
                AccountName = accountUpdatedEvent.Name,
                AccountStatus = AccountStatus.Active,
                CurrencyCode = ToCurrencyCode(accountUpdatedEvent.Currency),
                LatestChange = accountUpdatedEvent.EventReceived,
            };
            AccountService.UpdateAccount(newAccount);

            //No events to unpark as nothing waits for an update
        }

        public void HandleAccountSettledEvent(AccountEvent accountSettledEvent)
        {
            if (!ValidateAccountExists(accountSettledEvent.Id, out _))
            {
                TracingService.LogTrace($"Account with id: {accountSettledEvent.Id} not found, park event until account exists", this.GetType().Name);
                // TODO: Park event
            }

            // Account cannot be in status settled or closed before the first settled event so no need to check for anything further
            Account newAccount = new()
            {
                Id = accountSettledEvent.Id,
                AccountName = accountSettledEvent.Name,
                AccountStatus = AccountStatus.Settled,
                CurrencyCode = ToCurrencyCode(accountSettledEvent.Currency),
                LatestChange = accountSettledEvent.EventReceived,
            };
            AccountService.SettleAccount(newAccount);

            //TODO: Unpark waiting events
        }

        public void HandleAccountClosedEvent(AccountEvent accountClosedEvent)
        {
            if (!ValidateAccountExists(accountClosedEvent.Id, out var account))
            {
                TracingService.LogTrace($"Account with id: {accountClosedEvent.Id} not found, park event until account exists", this.GetType().Name);
                // TODO: Park event
            }

            if(account?.AccountStatus != AccountStatus.Settled)
            {
                TracingService.LogTrace($"Account with id: {accountClosedEvent.Id} is not settled yet, park event until account is settled", this.GetType().Name);
                // TODO: Park event
            }

            Account newAccount = new()
            {
                Id = accountClosedEvent.Id,
                AccountName = accountClosedEvent.Name,
                AccountStatus = AccountStatus.Settled,
                CurrencyCode = ToCurrencyCode(accountClosedEvent.Currency),
                LatestChange = accountClosedEvent.EventReceived,
            };
            AccountService.CloseAccount(newAccount);

            // No need to unpark events as account is closed and no more events can trigger
        }

        public bool ValidateAccountExists(int accountId, out Account? account)
        {
            var existingAccount = AccountService.GetAccount(accountId);
            if (existingAccount == null)
            {
                account = existingAccount;
                return false;
            }
            account = existingAccount;
            return true;
        }

        private static CurrencyCode ToCurrencyCode(string currency)
        {
            switch (currency)
            {
                case "USD":
                    return CurrencyCode.USD;
                case "DKK":
                    return CurrencyCode.DKK;
                case "EUR":
                    return CurrencyCode.EUR;
                case "GBP":
                    return CurrencyCode.GBP;
                case "NOK":
                    return CurrencyCode.NOK;
                default:
                    return CurrencyCode.USD;
                    
            }
        }
    }
}
