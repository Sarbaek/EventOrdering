using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountServiceTest
    {
        [Fact]
        public void Add_ShouldSaveDataInRepo()
        {
            // Arrange
            var accountService = new AccountService();
            var account = new Account(1, "Test", CurrencyCode.DKK, AccountStatus.Active);

            // Act
            accountService.AddAccount(account);

            // Assert
            Assert.Equal(account, accountService.AccountRepository[1]);

        }
    }
}