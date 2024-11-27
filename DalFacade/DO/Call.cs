using Microsoft.VisualBasic;

namespace DO;

public record Call
(
    int Id,
    string FullAddressCall,
    double Latitude,
    double Longitude,
    DateTime openTime,
    Enums.CallTypeEnum CallType,
    string? VerbalDescription = null,
    DateTime? MaxTimeFinish = null
)
{
    public Call() : this(0, "", 0, 0, new DateTime(), 0) { }
}