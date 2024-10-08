namespace EventOrdering
{
    public interface IAccountEventHandler
    {
        void HandleAccountCreatedEvent(AccountEvent accountCreatedEvent);
        void HandleAccountUpdatedEvent(AccountEvent accountUpdatedEvent);
        void HandleAccountSettledEvent(AccountEvent accountSettledEvent);
    }
}
