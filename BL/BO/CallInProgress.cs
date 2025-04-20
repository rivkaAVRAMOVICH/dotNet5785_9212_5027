using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallInProgress
    {
        public int Id { get; init; }
        public int IdCall { get; init; }
        public Enums.CallType CallType { get; set; }
        public string? CallDescription { get; set; }
        public string CallAddress { get; set; }
        public DateTime? MaxEndTime { get; set; }
        public double DistanceCallFromVolunteer { get; set; }
        public Enums.Status Status  { get; set; }
}
    }
}
