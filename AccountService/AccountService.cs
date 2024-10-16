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
            AccountRepository.TryAdd(account.Id, account);
        }

        public void CloseAccount(Account account)
        {
            //throw new NotImplementedException();
        }

        public void SettleAccount(Account account)
        {
            //throw new NotImplementedException();
        }

        public void UpdateAccount(Account account)
        {
            AccountRepository.TryGetValue(account.Id, out var existingItem);
            if(existingItem == null)
            {
                TracingService.LogError($"Account with id: {account.Id} does not exist, cannot update", this.GetType().Name);
                return;
            }
            AccountRepository.TryUpdate(account.Id, account, existingItem);
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
