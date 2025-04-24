using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; }
    public string CallAddress { get; }
    public DateTime StartCallTime { get; }
    public DateTime EntryCallTime { get; }
    public DateTime? EndCallTime { get; }
    public FinishType? FinishType { get; }
}
