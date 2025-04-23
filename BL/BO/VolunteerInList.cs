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
        public string Name { get; }
        public bool IsActive { get; }
        public int HandledCallsSum { get; }
        public int CanceledCallsSum { get; }
        public int ExpiredCallsSum { get; }
        public int? HandleCallId { get; }
        public  HandleCallType HandleCallType { get; }
    }
}
