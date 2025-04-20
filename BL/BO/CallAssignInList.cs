using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallAssignInList
    {
        public int? VolunteerId { get; init; }
        public string? VolunteerName { get; }
        public DateTime EntryCallTime { get; }
        public DateTime? EndCallTime { get; }
        public Enums.EndCallType? EndCallTime { get; }
    }
}
