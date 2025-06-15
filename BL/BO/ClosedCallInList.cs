using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string CallAddress { get; init; }
    public DateTime StartCallTime { get; init; }
    public DateTime EntryCallTime { get; init; }
    public DateTime? EndCallTime { get; init; }
    public FinishType? FinishType { get; init; }
    public CallType? Type { get; set; }

    public override string ToString() { return this.ToStringProperty(); }
}
