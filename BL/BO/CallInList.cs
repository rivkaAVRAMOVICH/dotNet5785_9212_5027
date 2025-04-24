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
        public CallType CallType { get; }
        public DateTime StartCallTime { get; }
        public TimeSpan? EndCallTimeSpan { get; }
        public string? LastVolunteerName { get; }
        public TimeSpan? CompleteTreatmentTimeSpan { get; }
        public Status Status { get; }
        public int AssignSum { get; }
    }
}
