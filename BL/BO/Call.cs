using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Call
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string? CallDescription { get; set; }
        public string? CallAddress { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime StartCallTime { get; set; }
        public DateTime? MaxEndCallTime { get; set; }
        public Status Status { get; set; }
        public TypeOfDistance TypeOfDistance { get; set; }
        public DistanceFromVolunteer DistanceFromVolunteer { get; set; }
        public List<BO.CallAssignInList>? CallAssignList { get; set; }
        public override string ToString() { return this.ToStringProperty(); }
    }
}
