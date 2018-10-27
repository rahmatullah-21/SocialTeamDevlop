using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Interfaces
{
    public interface IEvent
    {
        string EventId { get; set; }
        string EventName { get; set; }
        string OwnerId { get; set; }
    }
}
