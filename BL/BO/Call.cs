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
        public Enums.CallType CallType { get; set; }
        public string? CallDescription { get; set; }
        public string CallAddress { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime StartCallTime { get; set; }
        public DateTime? MaxEndCallTime { get; set; }
        public Enums.Status Status { get; set; }
        public List<BO.CallAssignInList>? CallAssignList { get; set; }
    }
}
