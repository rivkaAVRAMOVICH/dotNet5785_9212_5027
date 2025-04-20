namespace DO;

public record Assignment
(
   int Id,
   int CallId,
   int VolunteerId,
   DateTime EnteryTimeTreatment,
   DateTime? FinushTimeTreatment = null,
   Enums.finishTreatmentTypeEnum? FinishTreatmentType = 0
)
{
    public Assignment() : this(0, 0, 0, new DateTime()) { }
    
}