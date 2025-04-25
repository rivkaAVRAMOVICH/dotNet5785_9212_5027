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
        public int CallId { get; init; }
        public CallType CallType { get; set; }
        public string CallAddress { get; set; }
        public string? CallDescription { get; set; }
        public DateTime? MaxEndTime { get; set; }
        public DateTime? TimeCallMade {  get; set; }
        public DateTime? EntryTimeTreatment {  get; set; }
        public double DistanceCallFromVolunteer { get; set; }
        public Status Status  { get; set; }

    }
}
