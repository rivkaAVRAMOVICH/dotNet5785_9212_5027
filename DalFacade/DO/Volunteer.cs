using System;
using System.Data;
using System.Net;
using System.Xml.Linq;

namespace DO;
public record Volunteer
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Email,
    Boolean Active,
    RoleEnum Role,
    DistanceTypeEnum DistanceType,
    string? Password = null,
    string? FullAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    double? MaxDistance = null
)
{
    public Volunteer() : this(0, "", "", "", false, 0, 0,null,null,null,null,null) { }
}