using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventOrdering;
using System.Globalization;
using System.Text.Json;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
var services = new ServiceCollection();

// Register required services for DI
services.AddSingleton<IAccountService, AccountService>();
services.AddSingleton<IAccountEventHandler, AccountEventHandler>();
services.AddSingleton<ITracingService, TracingService>();

var serviceProvider = services.BuildServiceProvider();

// Simulate event stream
var eventStreamA = new List<Event>() { 
    MapEvent(EventType.AccountOpened, $"{{\"Timestamp\":\"1971-03-15T08:00:00Z\",\"Payload\":{{ \"Id\":1,\"Currency\":\"DKK\",\"Name\":\"Salary\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:01Z\",\"Payload\":{{ \"Id\":1,\"Currency\":\"DKK\",\"Name\":\"Salary 2\"}}}}"),
    MapEvent(EventType.AccountClosed, $"{{\"Timestamp\":\"1971-03-15T08:00:06Z\",\"Payload\":{{ \"Id\":1,\"Currency\":\"DKK\",\"Name\":\"Salary 2\"}}}}"),
    MapEvent(EventType.AccountOpened, $"{{\"Timestamp\":\"1971-03-15T08:00:02Z\",\"Payload\":{{ \"Id\":2,\"Currency\":\"EUR\",\"Name\":\"Savings\"}}}}"),
    MapEvent(EventType.AccountSettled, $"{{\"Timestamp\":\"1971-03-15T08:00:08Z\",\"Payload\":{{ \"Id\":2,\"Currency\":\"GBP\",\"Name\":\"My savings\"}}}}"),
    MapEvent(EventType.AccountOpened, $"{{\"Timestamp\":\"1971-03-15T08:00:10Z\",\"Payload\":{{ \"Id\":3,\"Currency\":\"NOK\",\"Name\":\"Inpayments\"}}}}"),
    MapEvent(EventType.AccountClosed, $"{{\"Timestamp\":\"1971-03-15T08:00:16Z\",\"Payload\":{{ \"Id\":3,\"Currency\":\"NOK\",\"Name\":\"MB payments\"}}}}"),
    MapEvent(EventType.AccountSettled, $"{{\"Timestamp\":\"1971-03-15T08:00:13Z\",\"Payload\":{{ \"Id\":3,\"Currency\":\"NOK\",\"Name\":\"MB payments\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:11Z\",\"Payload\":{{ \"Id\":4,\"Currency\":\"DKK\",\"Name\":\"Salary 37\"}}}}"),
    MapEvent(EventType.AccountSettled, $"{{\"Timestamp\":\"1971-03-15T08:00:14Z\",\"Payload\":{{ \"Id\":4,\"Currency\":\"DKK\",\"Name\":\"Salary 37\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:18Z\",\"Payload\":{{ \"Id\":5,\"Currency\":\"USD\",\"Name\":\"Salary 242\"}}}}"),
    MapEvent(EventType.AccountClosed, $"{{\"Timestamp\":\"1971-03-15T08:00:20Z\",\"Payload\":{{ \"Id\":5,\"Currency\":\"USD\",\"Name\":\"Salary 242\"}}}}"),
};

var eventStreamB = new List<Event>() {
    MapEvent(EventType.AccountSettled, $"{{\"Timestamp\":\"1971-03-15T08:00:04Z\",\"Payload\":{{ \"Id\":1,\"Currency\":\"DKK\",\"Name\":\"Salary 2\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:03Z\",\"Payload\":{{ \"Id\":2,\"Currency\":\"GBP\",\"Name\":\"Savings\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:07Z\",\"Payload\":{{ \"Id\":2,\"Currency\":\"GBP\",\"Name\":\"My savings\"}}}}"),
    MapEvent(EventType.AccountClosed, $"{{\"Timestamp\":\"1971-03-15T08:00:09Z\",\"Payload\":{{ \"Id\":2,\"Currency\":\"GBP\",\"Name\":\"My savings\"}}}}"),
    MapEvent(EventType.AccountUpdated, $"{{\"Timestamp\":\"1971-03-15T08:00:12Z\",\"Payload\":{{ \"Id\":3,\"Currency\":\"NOK\",\"Name\":\"MB payments\"}}}}"),
    MapEvent(EventType.AccountOpened, $"{{\"Timestamp\":\"1971-03-15T08:00:05Z\",\"Payload\":{{ \"Id\":4,\"Currency\":\"DKK\",\"Name\":\"Salary 42\"}}}}"),
    MapEvent(EventType.AccountClosed, $"{{\"Timestamp\":\"1971-03-15T08:00:17Z\",\"Payload\":{{ \"Id\":4,\"Currency\":\"DKK\",\"Name\":\"Salary 37\"}}}}"),
    MapEvent(EventType.AccountOpened, $"{{\"Timestamp\":\"1971-03-15T08:00:15Z\",\"Payload\":{{ \"Id\":5,\"Currency\":\"USD\",\"Name\":\"Salary 24\"}}}}"),
    MapEvent(EventType.AccountSettled, $"{{\"Timestamp\":\"1971-03-15T08:00:19Z\",\"Payload\":{{ \"Id\":5,\"Currency\":\"USD\",\"Name\":\"Salary 242\"}}}}"),
};

// Simulate two containers running eventhandling concurrently
var a = Task.Run(() => EventHandlerTask.HandleEvents(serviceProvider.GetRequiredService<IAccountEventHandler>(), serviceProvider.GetRequiredService<ITracingService>(), "Pod A", eventStreamA));
var b = Task.Run(() => EventHandlerTask.HandleEvents(serviceProvider.GetRequiredService<IAccountEventHandler>(), serviceProvider.GetRequiredService<ITracingService>(), "Pod B", eventStreamB));


Console.ReadLine();


// For testing purposes only. Maps the json string from the assignment to an event object
static Event MapEvent(EventType eventType, string jsonString)
{
    var deserializedEvent = JsonSerializer.Deserialize<Event>(jsonString);
    if (deserializedEvent == null)
    {
        throw new Exception("Error in deserialized data");
    }
    deserializedEvent.EventType = eventType;

    return deserializedEvent;
}