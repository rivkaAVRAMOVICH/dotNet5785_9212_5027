using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; init; }
        public string? CallDescription { get; init; }
        public string CallAddress { get; init; }
        public DateTime StartCallTime { get; init; }
        public DateTime? MaxEndCallTime { get; init; }
        public double DistanceCallFromVolunteer { get; set; }
        public override string ToString() { return this.ToStringProperty(); }
    }
}
