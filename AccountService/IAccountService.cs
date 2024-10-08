using EventOrdering.DomainModel;

namespace EventOrdering
{
    public interface IAccountService
    {
        void AddAccount(Account account);
        void UpdateAccount(Account account);

        void SettleAccount(int id);

        void CloseAccount(int id);
    }
}
