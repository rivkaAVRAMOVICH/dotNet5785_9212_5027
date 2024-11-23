namespace DO;

public record Assignment
(
   int Id,
   int CallId,
   int VolunteerId,
   DateTime EnteryTimeTreatment,
   DateTime? FinushTimeTreatment = null,
   Enum? finishTreatmentType = null
)
{
    public Assignment() : this(0) { }
}