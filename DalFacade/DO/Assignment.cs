namespace DO;

public record Assignment
(
   int Id,
   int CallId,
   int VolunteerId,
   DateTime EntryTimeTreatment,
   DateTime? FinishTimeTreatment = null,
   EndTypeAssignment? EndTypeAssignment = 0
)
{
    public Assignment() : this(0, 0, 0, new DateTime()) { }
    
}