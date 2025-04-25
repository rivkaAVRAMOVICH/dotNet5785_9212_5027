using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace Helpers;
using DalApi;
using DO;

/// <summary>
/// Provides methods to manage calls and assignments within the system.
/// Includes methods for retrieving statuses, geocoding, calculating distances, and more.
/// </summary>
internal class CallManager
{
    private static IDal _dal = Factory.Get;

    /// <summary>
    /// Determines the status of a given call
    /// </summary>
    /// <param name="call">The call to evaluate</param>
    /// <returns>The status of the call</returns>
    internal static BO.Status CallStatus(DO.Call call)
    {
        var currentTime = DateTime.Now;

        // Check for an open assignment related to the call
        var openAssignment = _dal.Assignment.ReadAll().FirstOrDefault(item => item.CallId == call.Id && item.EndTime == null);
        if (openAssignment != null)
        {
            return BO.Status.inProgress;
        }

        // Check if the call has expired
        if (call.MaxEndCallTime < currentTime)
        {
            return BO.Status.expired;
        }

        // Check for a closed assignment related to the call
        var closedAssignment = _dal.Assignment.ReadAll().FirstOrDefault(item => item.CallId == call.Id && item.EndTime != null);
        if (closedAssignment != null)
        {
            return BO.Status.closed;
        }

        // Check if the call is at risk of expiring while in progress
        if (call.MaxEndCallTime != null && call.StartCallTime.AddHours(1) < currentTime)
        {
            return BO.Status.inProgressAtRisk;
        }

        // Check if the call is open but at risk of expiring
        if (call.MaxEndCallTime < currentTime.AddHours(1))
        {
            return BO.Status.openAtRisk;
        }

        // Default to the call being open
        return BO.Status.open;
    }

    /// <summary>
    /// Retrieves latitude and longitude for a given address using the LocationIQ API
    /// </summary>
    /// <param name="address">The address to geocode</param>
    /// <returns>A tuple containing latitude and longitude of the address</returns>
    /// <exception cref="BO.BlNullPropertyException">Thrown when the provided address is null or empty</exception>
    /// <exception cref="BO.BlUnexpectedSystemException">Thrown when there is an error during the API call or the response cannot be processed</exception>
    public static (double Latitude, double Longitude) GetLatitudLongitute(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new BO.BlNullPropertyException("Address cannot be empty");

        using (var client = new HttpClient())
        {
            var requestUri = "https://us1.locationiq.com/v1/reverse?key=Your_API_Access_Token&lat=51.525460&lon=-0.15222855&format=json&";

            // Perform a synchronous HTTP GET request
            var response = client.GetAsync(requestUri).Result;

            if (!response.IsSuccessStatusCode)
                throw new BO.BlUnexpectedSystemException($"Error fetching coordinates: {response.ReasonPhrase}");

            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            // Parse the JSON response
            var locationData = JsonConvert.DeserializeObject<List<LocationIqResponse>>(jsonResponse);

            if (locationData == null || locationData.Count == 0)
                throw new BO.BlUnexpectedSystemException("Unable to find coordinates for the specified address.");

            var firstResult = locationData[0];
            return (double.Parse(firstResult.Lat), double.Parse(firstResult.Lon));
        }
    }

    /// <summary>
    /// Calculates the distance between two geographical points using the Haversine formula
    /// </summary>
    /// <param name="lat1">Latitude of the first point</param>
    /// <param name="lon1">Longitude of the first point</param>
    /// <param name="lat2">Latitude of the second point</param>
    /// <param name="lon2">Longitude of the second point</param>
    /// <returns>The distance in kilometers between the two points</returns>
    public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers

        // Convert degree differences to radians
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        // Apply the Haversine formula
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    /// <summary>
    /// Converts degrees to radians
    /// </summary>
    /// <param name="degrees">The angle in degrees</param>
    /// <returns>The angle in radians</returns>
    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    /// <summary>
    /// Represents a response object from the LocationIQ API
    /// </summary>
    private class LocationIqResponse
    {
        [JsonProperty("lat")]
        public string Lat { get; set; }

        [JsonProperty("lon")]
        public string Lon { get; set; }
    }

    /// <summary>
    /// Updates calls that have expired by creating or updating assignments accordingly
    /// </summary>
    public static void UpdateExpiredCalls()
    {
        var currentTime = ClockManager.Now;

        // Retrieve calls with no assignments whose time has expired
        var callsWithNoAssignments = _dal.Call.ReadAll()
            .Where(call => call.MaxEndCallTime <= currentTime && CallManager.CallStatus(call) != BO.Status.closed && !_dal.Assignment.ReadAll().Any(a => a.CallId == call.Id));

        // Retrieve calls with existing assignments but no actual end time
        var callsThatHaveAssignments = _dal.Call.ReadAll()
            .Where(call => call.MaxEndCallTime <= currentTime && CallManager.CallStatus(call) != BO.Status.closed && _dal.Assignment.ReadAll().Any(a => a.CallId == call.Id && a.EndTime == null));

        // Handle calls with no assignments
        foreach (var call in callsWithNoAssignments)
        {
            _dal.Assignment.Create(new Assignment
            {
                Id = 0,
                CallId = call.Id,
                VolunteerId = 0,
                StartTime = currentTime,
                EndTime = currentTime,
                FinishType = DO.FinishType.expired
            });
        }

        // Handle calls with existing assignments
        foreach (var call in callsThatHaveAssignments)
        {
            var tmpAssignment = _dal.Assignment.ReadAll().First(a => a.CallId == call.Id && a.EndTime == null);
            _dal.Assignment.Update(new Assignment
            {
                Id = tmpAssignment.Id,
                CallId = tmpAssignment.CallId,
                VolunteerId = tmpAssignment.VolunteerId,
                StartTime = tmpAssignment.StartTime,
                EndTime = currentTime,
                FinishType = DO.FinishType.expired
            });
        }
    }

    /// <summary>
    /// Validates whether a volunteer is authorized to finish an assignment
    /// </summary>
    /// <param name="id">The volunteer's ID</param>
    /// <param name="assignment">The assignment to validate</param>
    /// <exception cref="BO.BlUnauthorizedActionException">Thrown when the assignment does not belong to the specified volunteer</exception>
    /// <exception cref="BO.BlInvalidAssignmentException">Thrown when the assignment has already been completed or is no longer active</exception>
    public static void ValidateAssignmentToFinish(int id, DO.Assignment assignment)
    {
        // Check authorization - the volunteer must own the assignment
        if (assignment.VolunteerId != id)
        {
            throw new BO.BlUnauthorizedActionException("The assignment does not belong to the specified volunteer");
        }

        // Check if the assignment is still active
        if (assignment.EndTime != null)
        {
            throw new BO.BlInvalidAssignmentException("The assignment has already been completed or is no longer active");
        }
    }

    /// <summary>
    /// Validates whether a volunteer or manager is authorized to cancel an assignment
    /// </summary>
    /// <param name="id">The requester's ID</param>
    /// <param name="assignment">The assignment to validate</param>
    /// <exception cref="BO.BlUnauthorizedActionException">Thrown when the requester does not have permission to cancel the assignment</exception>
    /// <exception cref="BO.BlInvalidAssignmentException">Thrown when the assignment has already been completed or is no longer active</exception>
    public static void ValidateAssignmentToCancel(int id, DO.Assignment assignment)
    {
        // Authorization check: manager or volunteer assigned to the task
        bool isAuthorized = (assignment.VolunteerId == id || _dal.Volunteer.Read(id)!.Role == DO.Role.manager);
        if (!isAuthorized)
        {
            throw new BO.BlUnauthorizedActionException("The requester does not have permission to cancel this assignment");
        }

        // Check if the assignment is still active
        if (assignment.EndTime != null)
        {
            throw new BO.BlInvalidAssignmentException("The assignment has already been completed or is no longer active");
        }
    }

    /// <summary>
    /// Converts a list of DO.Call objects into a list of BO.CallInList objects
    /// </summary>
    /// <param name="calls">The list of DO.Call objects to convert</param>
    /// <returns>A list of BO.CallInList objects</returns>
    public static IEnumerable<BO.CallInList> ConvertCallsToBO(IEnumerable<DO.Call> calls)
    {
        return calls.Select(call => new BO.CallInList
        {
            Id = call.Id,
            CallId = call.Id,
            CallType = (BO.CallType)call.CallType,
            StartCallTime = call.StartCallTime,
            RemainingTime = call.MaxEndCallTime.HasValue ? call.MaxEndCallTime.Value - DateTime.Now : null,
            LastVolunteer = null,
            CompletionTime = null,
            Status = BO.Status.open,
            Assignments = 0
        });
    }

    /// <summary>
    /// Retrieves the value of a specific field from a BO.CallInList object
    /// </summary>
    /// <param name="call">The call to retrieve the field from</param>
    /// <param name="field">The field to retrieve</param>
    /// <returns>The value of the specified field, or null if not found</returns>
    public static object? GetFieldValue(BO.CallInList call, Enum field)
    {
        return field switch
        {
            BO.CallType => call.CallType,
            BO.Status => call.Status,
            _ => null
        };
    }

    /// <summary>
    /// Validates the properties of a BO.Call object
    /// </summary>
    /// <param name="call">The call to validate</param>
    /// <exception cref="BO.BlValidationException">Thrown when the maximum allowed time is earlier than or equal to the time the call was made</exception>
    /// <exception cref="BO.BlNullPropertyException">Thrown when the address is null or empty</exception>
    public static void ValidateCall(BO.Call call)
    {
        // Ensure the maximum time is after the call's creation time
        if (call.MaxEndCallTime <= call.StartCallTime)
        {
            throw new BO.BlValidationException("Max time must be later than the time the call was made");
        }

        // Ensure the address is not empty or null
        if (string.IsNullOrWhiteSpace(call.CallAddress))
        {
            throw new BO.BlNullPropertyException("Address cannot be null or empty");
        }
    }

    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        throw new NotImplementedException();
    }

    internal static void SimulateCourseRegistrationAndGrade()
    {
        throw new NotImplementedException();
    }
}
