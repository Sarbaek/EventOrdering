using EventOrdering.DomainModel;

namespace EventOrdering
{
    public interface IAccountService
    {
        void AddAccount(Account account);
        void UpdateAccount(Account account);

        void SettleAccount(Account account);

        void CloseAccount(Account account);

        Account? GetAccount(int id);
    }
}
