namespace EventOrdering
{
    public interface IAccountEventHandler
    {
        void HandleAccountOpenedEvent(AccountEvent accountOpenedEvent);
        void HandleAccountUpdatedEvent(AccountEvent accountUpdatedEvent);
        void HandleAccountSettledEvent(AccountEvent accountSettledEvent);
        void HandleAccountClosedEvent(AccountEvent accountClosedEvent);
    }
}
