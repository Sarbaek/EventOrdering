using System.Collections.Concurrent;

namespace EventOrdering
{
    public class ParkingService(ITracingService tracingService): IParkingService
    {
        public static ConcurrentDictionary<(int, EventType), AccountEvent> ParkedEvents { get; set; } = new ConcurrentDictionary<(int, EventType), AccountEvent>();
        protected ITracingService TracingService { get; set; } = tracingService;

        public void ParkEvent(int id, EventType eventToWaitFor, AccountEvent accountEvent)
        {
            TracingService.LogTrace($"Parking event: {id}, {eventToWaitFor}", this.GetType().Name);
            lock (ParkedEvents)
            {
                ParkedEvents.TryAdd((id, eventToWaitFor), accountEvent);
            }
        }

        //UnparkEvent should really add the event to the event stream again instead of returning it, but since the event stream is abstracted out, this is the approach that's used
        public AccountEvent UnparkEvent(int id, EventType triggeringEvent)
        {
            lock (ParkedEvents)
            {
                ParkedEvents.TryRemove((id, triggeringEvent), out var accountEvent);

                if (accountEvent == null)
                {
                    TracingService.LogTrace("No parked event found", this.GetType().Name);
                }
#pragma warning disable CS8603 // Possible null reference return. Returning null reference on purpose when no parked event is found
                return accountEvent;
#pragma warning restore CS8603 // Possible null reference return.
            }
        }
    }
}
