namespace DO;

public record Volunteer
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Email,
    Boolean Active,
    Enum Role,
    Enum DistanceType,
    string? Password = null,
    string? FullAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    double? MaxDistance = null
)
{
    public Volunteer() : this(0) { }
}