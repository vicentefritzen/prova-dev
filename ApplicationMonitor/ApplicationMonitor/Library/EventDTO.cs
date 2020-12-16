using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationMonitor.Library
{
    public class EventDTO
    {
        public DateTime EventDate { get; set; }
        public int PanelId { get; set; }
        public int AccountId { get; set; }
        public int EventId{ get; set; }
        public string EventDescription { get; set; }
        public int PartitionId { get; set; }
        public int ZoneId { get; set; }
        public int UserId { get; set; }

    }
}
