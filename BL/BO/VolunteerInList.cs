using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class VolunteerInList
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int SumOfHandleCalls { get; set; }
        public int SumCanceledCalls { get; set; }
        public int SumOfExpiredCalls { get; set; }
        public int? IdCallInProgress { get; set; }
        public  HandleCallType TypeCallInProgress { get; set; }
    }
}
