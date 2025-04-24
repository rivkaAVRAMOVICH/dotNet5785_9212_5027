using Microsoft.VisualBasic;

namespace DO;

public record Call
(
    int Id,
    string CallAddress,
    double Latitude,
    double Longitude,
    DateTime StartCallTime,
    CallType CallType,
    string? CallDescription = null,
    DateTime? MaxEndCallTime = null
)
{
    public Call() : this(0, "", 0, 0, new DateTime(), 0) { }
}