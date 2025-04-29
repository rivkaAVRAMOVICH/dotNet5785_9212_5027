using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class CallInList
    {
        public int? Id { get; init; }
        public int CallId { get; init; }
        public CallType CallType { get; set; }
        public DateTime StartCallTime { get; set; }
        public TimeSpan? EndCallTimeSpan { get; set; }
        public string? LastVolunteerName { get; set; }
        public TimeSpan? CompleteTreatmentTimeSpan { get; set; }
        public Status Status { get; set; }
        public int AssignSum { get; set; }
        public override string ToString() { return this.ToStringProperty(); }
    }
}
