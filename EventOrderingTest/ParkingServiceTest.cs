using EventOrdering.DomainModel;
using FluentAssertions;

namespace EventOrdering
{
    public class ParkingServiceTest
    {
        [Fact]
        public void ParkEvent_ShouldAddParkedEvent()
        {
            var parkingService = SetupTestData(out var accountEvent);

            parkingService.ParkEvent(1, EventType.AccountSettled, accountEvent);

            ParkingService.ParkedEvents.Count.Should().Be(1);

            TearDownTest();
        }

        [Fact]
        public void UnparkEvent_ShouldRemoveParkedEvent()
        {
            var parkingService = SetupTestData(out var accountEvent);
            parkingService.ParkEvent(1, EventType.AccountSettled, accountEvent);

            var parkedEvent = parkingService.UnparkEvent(1, EventType.AccountSettled);

            parkedEvent.Should().NotBeNull();
            parkedEvent.Id.Should().Be(1);
            parkedEvent.Name.Should().Be("TestEvent");
            parkedEvent.Currency.Should().Be("USD");
            parkedEvent.EventType.Should().Be(EventType.AccountClosed);
            ParkingService.ParkedEvents.Count.Should().Be(0);

            TearDownTest();
        }

        [Fact]
        public void UnparkEvent_ShouldNotRemoveParkedEvent_IdMismatch()
        {
            var parkingService = SetupTestData(out var accountEvent);
            parkingService.ParkEvent(1, EventType.AccountSettled, accountEvent);

            var parkedEvent = parkingService.UnparkEvent(2, EventType.AccountSettled);

            parkedEvent.Should().BeNull();
            ParkingService.ParkedEvents.Count.Should().Be(1);

            TearDownTest();
        }

        [Fact]
        public void UnparkEvent_ShouldNotRemoveParkedEvent_EventTypeMismatch()
        {
            var parkingService = SetupTestData(out var accountEvent);
            parkingService.ParkEvent(2, EventType.AccountSettled, accountEvent);

            var parkedEvent = parkingService.UnparkEvent(2, EventType.AccountOpened);

            parkedEvent.Should().BeNull();
            ParkingService.ParkedEvents.Count.Should().Be(1);

            TearDownTest();
        }

        private IParkingService SetupTestData(out AccountEvent accountEvent)
        {
            var tracingService = new TracingService();
            var parkingService = new ParkingService(tracingService);
            accountEvent = new AccountEvent(DateTime.UtcNow, DateTime.UtcNow, 1, "USD", "TestEvent", EventType.AccountClosed);

            return parkingService;
        }

        private void TearDownTest()
        {
            lock (ParkingService.ParkedEvents)
            {
                ParkingService.ParkedEvents.Clear();
            }
        }
    }
}