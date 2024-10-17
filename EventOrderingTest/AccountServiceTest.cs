using EventOrdering.DomainModel;
using FluentAssertions;

namespace EventOrdering
{
    public class AccountServiceTest
    {
        [Fact]
        public void Add_ShouldSaveDataInRepo()
        {
            var accountService = SetupTest();
            var account = new Account()
            {
                Id = 1,
                AccountName = "Test",
                AccountStatus = AccountStatus.Active,
                CurrencyCode = CurrencyCode.USD,
                LatestChange = DateTime.UtcNow,
            };

            accountService.AddAccount(account);

            accountService.AccountRepository.Should().HaveCount(1);
        }

        [Fact]
        public void Update_ShouldUpdateDataInRepo()
        {
            var accountService = SetupTest();
            var account = new Account()
            {
                Id = 2,
                AccountName = "Test",
                AccountStatus = AccountStatus.Active,
                CurrencyCode = CurrencyCode.USD,
                LatestChange = DateTime.UtcNow,
            };
            accountService.AddAccount(account);
            account.AccountName = "Test but updated";

            accountService.UpdateAccount(account);

            accountService.AccountRepository.TryGetValue(account.Id, out var retrievedAccount);
            retrievedAccount.Should().NotBeNull();
            retrievedAccount?.AccountName.Should().Be(account.AccountName);
        }

        [Fact]
        public void Settle_ShouldSettleAccount()
        {
            var accountService = SetupTest();
            var account = new Account()
            {
                Id = 3,
                AccountName = "Test",
                AccountStatus = AccountStatus.Active,
                CurrencyCode = CurrencyCode.USD,
                LatestChange = DateTime.UtcNow,
            };
            accountService.AddAccount(account);

            accountService.SettleAccount(account);

            accountService.AccountRepository.TryGetValue(account.Id, out var retrievedAccount);
            retrievedAccount.Should().NotBeNull();
            retrievedAccount?.AccountStatus.Should().Be(AccountStatus.Settled);
        }

        [Fact]
        public void Close_ShouldCloseAccount()
        {
            var accountService = SetupTest();
            var account = new Account()
            {
                Id = 4,
                AccountName = "Test",
                AccountStatus = AccountStatus.Settled,
                CurrencyCode = CurrencyCode.USD,
                LatestChange = DateTime.UtcNow,
            };
            accountService.AddAccount(account);

            accountService.CloseAccount(account);

            accountService.AccountRepository.TryGetValue(account.Id, out var retrievedAccount);
            retrievedAccount.Should().NotBeNull();
            retrievedAccount?.AccountStatus.Should().Be(AccountStatus.Closed);
        }

        private IAccountService SetupTest()
        {
            var tracingService = new TracingService();
            var accountService = new AccountService(tracingService);
            return accountService;
        }
    }
}