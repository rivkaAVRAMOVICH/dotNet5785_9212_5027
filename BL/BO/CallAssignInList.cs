using Helpers;

namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? VolunteerName { get; init; }
    public DateTime EntryCallTime { get; init; }
    public DateTime? EndCallTime { get; init; }
    public FinishType? FinishType { get; init; }
    public override string ToString() { return this.ToStringProperty(); }
}
