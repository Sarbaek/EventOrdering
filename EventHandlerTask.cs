using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventOrdering
{
    public static class EventHandlerTask
    {
        public static async Task HandleEvents(IAccountEventHandler accountEventHandler, string podName)
        {
            await Task.Delay(1000);
            Console.WriteLine($"Handling events for pod {podName}");
        }
    }
}
