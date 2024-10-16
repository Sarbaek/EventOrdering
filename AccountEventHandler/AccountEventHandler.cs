using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountEventHandler(IAccountService accountService, ITracingService tracingService, IParkingService parkingService) : IAccountEventHandler
    {
        public IAccountService AccountService { get; set; } = accountService;
        public ITracingService TracingService { get; set; } = tracingService;
        public IParkingService ParkingService { get; set; } = parkingService;

        public void HandleAccountOpenedEvent(AccountEvent accountOpenedEvent)
        {
            if (ValidateAccountExists(accountOpenedEvent.Id, out _))
            {
                TracingService.LogError($"Received account opened event but account already exists. Id: {accountOpenedEvent.Id}", GetType().Name);
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

            //Unpark waiting events
            var parkedEvent = ParkingService.UnparkEvent(accountOpenedEvent.Id, EventType.AccountOpened);
            if (parkedEvent == null)
            {
                TracingService.LogTrace("No parked events found, continuing processing events", GetType().Name);
                return;
            }
            HandleParkedEvent(parkedEvent);
        }

        public void HandleAccountUpdatedEvent(AccountEvent accountUpdatedEvent)
        {
            var existingAccount = AccountService.GetAccount(accountUpdatedEvent.Id);

            // Handle update event arriving before opened event
            if (!ValidateAccountExists(accountUpdatedEvent.Id, out var account))
            {
                TracingService.LogError($"Account with id: {accountUpdatedEvent.Id} does not exist, parking event until account opened event arrives", GetType().Name);
                ParkingService.ParkEvent(accountUpdatedEvent.Id, EventType.AccountOpened, accountUpdatedEvent);
                return;
            }

            // Handle update event arriving late to the party
            if(account?.LatestChange > accountUpdatedEvent.EventReceived)
            {
                TracingService.LogError($"Account with id: {accountUpdatedEvent.Id} already updated by later event, no more work needed", GetType().Name);
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
                TracingService.LogTrace($"Account with id: {accountSettledEvent.Id} not found, park event until account exists", GetType().Name);
                ParkingService.ParkEvent(accountSettledEvent.Id, EventType.AccountOpened, accountSettledEvent);
                return;
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

            //Unpark waiting events
            var parkedEvent = ParkingService.UnparkEvent(accountSettledEvent.Id, EventType.AccountSettled);
            if (parkedEvent == null)
            {
                TracingService.LogTrace("No parked events found, continuing processing events", GetType().Name);
                return;
            }
            HandleParkedEvent(parkedEvent);
        }

        public void HandleAccountClosedEvent(AccountEvent accountClosedEvent)
        {
            if (!ValidateAccountExists(accountClosedEvent.Id, out var account))
            {
                TracingService.LogTrace($"Account with id: {accountClosedEvent.Id} not found, park event until account exists", GetType().Name);
                // Park event until AccountOpened event arrives
                ParkingService.ParkEvent(accountClosedEvent.Id, EventType.AccountOpened, accountClosedEvent);
                return;
            }

            if(account?.AccountStatus != AccountStatus.Settled)
            {
                TracingService.LogTrace($"Account with id: {accountClosedEvent.Id} is not settled yet, park event until account is settled", GetType().Name);
                // Park even until AccountSettled event arrives
                ParkingService.ParkEvent(accountClosedEvent.Id, EventType.AccountSettled, accountClosedEvent);
                return;
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

        protected void HandleParkedEvent(AccountEvent parkedEvent)
        {
            switch (parkedEvent.EventType)
            {
                case EventType.AccountOpened:
                    HandleAccountOpenedEvent(parkedEvent);
                    break;
                case EventType.AccountClosed:
                    HandleAccountClosedEvent(parkedEvent);
                    break;
                case EventType.AccountUpdated:
                    HandleAccountUpdatedEvent(parkedEvent);
                    break;
                case EventType.AccountSettled:
                    HandleAccountSettledEvent(parkedEvent);
                    break;
                default:
                    TracingService.LogError("Parked event type not found", GetType().Name);
                    break;
            }
        }
    }
}
