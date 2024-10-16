using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventOrdering
{
    public static class EventHandlerTask
    {
        public static async Task HandleEvents(IAccountEventHandler accountEventHandler, ITracingService tracingService, string podName, List<Event> eventStream)
        {
            await Task.Delay(1000);
            tracingService.LogTrace("Handling events", podName);
            foreach (Event e in eventStream)
            {
                tracingService.LogTrace($"Received event of type: {e.EventType}", podName);
                switch (e.EventType)
                {
                    case EventType.AccountOpened:
                        tracingService.LogTrace($"Handling event of type: ${e.EventType}", podName);
                        accountEventHandler.HandleAccountOpenedEvent(MapEvent(e, tracingService));
                        break;
                    case EventType.AccountUpdated:
                        tracingService.LogTrace($"Handling event of type: ${e.EventType}", podName);
                        accountEventHandler.HandleAccountUpdatedEvent(MapEvent(e, tracingService));
                        break;
                    case EventType.AccountSettled:
                        tracingService.LogTrace($"Handling event of type: ${e.EventType}", podName);
                        accountEventHandler.HandleAccountSettledEvent(MapEvent(e, tracingService));
                        break;
                    case EventType.AccountClosed:
                        tracingService.LogTrace($"Handling event of type: ${e.EventType}", podName);
                        accountEventHandler.HandleAccountClosedEvent(MapEvent(e, tracingService));
                        break;
                    default:
                        tracingService.LogError("Event type not found!", podName);
                        break;
                }
            }
        }

        public static AccountEvent MapEvent(Event receivedEvent, ITracingService tracingService) 
        {
            if(receivedEvent.Payload == null)
            {
                tracingService.LogError("Payload is empty", "MapEvent");
                throw new ArgumentException("Payload is empty");
            }
            return new AccountEvent(DateTime.Now, receivedEvent.Timestamp, receivedEvent.Payload.Id, receivedEvent.Payload.Currency, receivedEvent.Payload.Name);
        }
    }
}
