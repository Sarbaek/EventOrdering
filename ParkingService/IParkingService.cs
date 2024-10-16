namespace EventOrdering
{
    public interface IParkingService
    {
        public AccountEvent UnparkEvent(int id, EventType triggeringEvent);
        public void ParkEvent(int id, EventType eventToWaitFor, AccountEvent accountEvent);
    }
}
