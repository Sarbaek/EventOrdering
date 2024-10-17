using EventOrdering.DomainModel;
using System.Collections.Concurrent;

namespace EventOrdering
{
    public interface IAccountService
    {
        public ConcurrentDictionary<int, Account> AccountRepository { get; set; }
        void AddAccount(Account account);
        void UpdateAccount(Account account);

        void SettleAccount(Account account);

        void CloseAccount(Account account);

        Account? GetAccount(int id);
    }
}
