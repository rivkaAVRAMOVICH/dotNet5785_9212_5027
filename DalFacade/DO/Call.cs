namespace DO;

public record Call
(
    int Id,
    string FullAddressCall,
    double Latitude,
    double Longitude,
    DateTime openTime,
    Enum CallType,
    string? VerbalDescription = null,
    DateTime? MaxTimeFinish = null
)
{
    public Call() : this(0) { }
}