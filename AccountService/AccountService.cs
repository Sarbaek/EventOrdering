using EventOrdering.DomainModel;
using System.Collections.Concurrent;

namespace EventOrdering
{
    public class AccountService(ITracingService tracingService) : IAccountService
    {
        protected ConcurrentDictionary<int, Account> AccountRepository { get; set; } = [];
        protected ITracingService TracingService { get; set; } = tracingService;

        public void AddAccount(Account account)
        {
            lock (AccountRepository)
            {
                AccountRepository.TryAdd(account.Id, account);
            }
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
                TracingService.LogError($"Account with id: {account.Id} does not exist, cannot update", this.GetType().Name);
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
            TracingService.LogTrace($"Account with id {id} not found", this.GetType().Name);
            return null;
        }
    }
}
