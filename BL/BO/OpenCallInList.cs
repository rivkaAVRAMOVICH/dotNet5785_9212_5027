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
        public Enums.CallType CallType { get; }
        public string? CallDescription { get; }
        public string CallAddress { get; }
        public DateTime StartCallTime { get; }
        public DateTime? MaxEndCallTime { get; }
        public double DistanceCallFromVolunteer { get; set; }
    }
}
