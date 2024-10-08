using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventOrdering
{
    public class AccountEvent(DateTime eventReceived, DateTime eventProcessed, int id, string currency, string name)
    {
        public DateTime EventReceived { get; set; } = eventReceived;
        public DateTime EventProcessed { get; set; } = eventProcessed;
        public int Id { get; set; } = id;
        public string Currency { get; set; } = currency;
        public string Name { get; set; } = name;
    }
}
