using EventOrdering.DomainModel;
using System.Collections.Concurrent;

namespace EventOrdering
{
    public class AccountService(ITracingService tracingService) : IAccountService
    {
        protected static ConcurrentDictionary<int, Account> AccountRepository { get; set; } = [];
        protected ITracingService TracingService { get; set; } = tracingService;

        public void AddAccount(Account account)
        {
            lock (AccountRepository)
            {
                TracingService.LogTrace($"Adding Account with id: {account.Id}", GetType().Name);
                AccountRepository.TryAdd(account.Id, account);
            }
            LogState();
        }

        public void CloseAccount(Account account)
        {
            account.AccountStatus = AccountStatus.Closed;
            UpdateAccount(account);
        }

        public void SettleAccount(Account account)
        {
            account.AccountStatus = AccountStatus.Settled;
            UpdateAccount(account);
        }

        public void UpdateAccount(Account account)
        {
            AccountRepository.TryGetValue(account.Id, out var existingItem);
            if(existingItem == null)
            {
                TracingService.LogError($"Account with id: {account.Id} does not exist, cannot update", GetType().Name);
                return;
            }
            lock (AccountRepository)
            {
                AccountRepository.TryUpdate(account.Id, account, existingItem);
            }
        }

        public Account? GetAccount(int id)
        {
            AccountRepository.TryGetValue(id, out var account);
            if (account != null)
            {
                return account;
            }
            TracingService.LogTrace($"Account with id {id} not found", GetType().Name);
            return null;
        }

        protected void LogState()
        {
            foreach (var account in AccountRepository.Values)
            {
                Console.WriteLine(account);
            }
        }
    }
}
