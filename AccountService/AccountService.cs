using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountService : IAccountService
    {
        public Dictionary<int, Account> AccountRepository { get; set; } = [];

        public void AddAccount(Account account)
        {
            AccountRepository.Add(account.Id, account);
        }

        public void CloseAccount(int id)
        {
            throw new NotImplementedException();
        }

        public void SettleAccount(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccount(Account account)
        {
            AccountRepository[account.Id] = account;
        }
    }
}
