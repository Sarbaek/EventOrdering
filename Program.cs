using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EventOrdering;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
var services = new ServiceCollection();

services.AddSingleton<IAccountService, AccountService>();
services.AddSingleton<IAccountEventHandler, AccountEventHandler>();

var serviceProvider = services.BuildServiceProvider();

var a = Task.Run(() => EventHandlerTask.HandleEvents(serviceProvider.GetRequiredService<IAccountEventHandler>(), "A"));
var b = Task.Run(() => EventHandlerTask.HandleEvents(serviceProvider.GetRequiredService<IAccountEventHandler>(), "B"));

await a;
await b;
Console.ReadLine();