using EventOrdering.DomainModel;

namespace EventOrdering
{
    public class AccountServiceTest
    {
        [Fact]
        public void Add_ShouldSaveDataInRepo()
        {
            // Arrange
            
            // Act
            
            // Assert
        }

        [Fact]
        public void Update_ShouldUpdateDataInRepo()
        {

        }

        [Fact]
        public void Update_ShouldWaitForCreateEvent()
        {

        }

        [Fact]
        public void Settle_ShouldWaitForCreateEvent()
        {

        }

        [Fact]
        public void Settle_ShouldSettleAccount()
        {

        }

        [Fact]
        public void Close_ShouldWaitForSettleEvent()
        {

        }

        [Fact]
        public void Close_ShouldCloseAccount()
        {

        }
    }
}