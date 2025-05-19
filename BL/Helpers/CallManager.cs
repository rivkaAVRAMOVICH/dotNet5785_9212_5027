using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
namespace Helpers;

using BO;
//using Newtonsoft.Json;
using DalApi;
using DO;

/// <summary>
/// Provides methods to manage calls and assignments within the system.
/// Includes methods for retrieving statuses, geocoding, calculating distances, and more.
/// </summary>
internal class CallManager
{
    private static IDal _dal = Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Determines the status of a given call
    /// </summary>
    /// <param name="call">The call to evaluate</param>
    /// <returns>The status of the call</returns>
    internal static BO.Status CallStatus(DO.Call call)
    {
        var currentTime = DateTime.Now;

        // Check for an open assignment related to the call
        var openAssignment = _dal.Assignment.ReadAll().FirstOrDefault(item => item.CallId == call.Id && item.FinishTimeTreatment == null);
        if (openAssignment != null)
        {
            if (call.MaxEndCallTime != null && call.StartCallTime.AddHours(1) < currentTime)
            {
                return BO.Status.inProgressAtRisk;
            }
            return BO.Status.inProgress;
        }

        if (call.MaxEndCallTime != null)
        {
            // Check if the call has expired
            if (call.MaxEndCallTime < currentTime)
            {
                return BO.Status.expired;
            }
        }
        // Check for a closed assignment related to the call
        var closedAssignment = _dal.Assignment.ReadAll().FirstOrDefault(item => item.CallId == call.Id && item.FinishTimeTreatment != null);
        if (closedAssignment != null)
        {
            return BO.Status.closed;
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
    /// Updates calls that have expired by creating or updating assignments accordingly
    /// </summary>
    public static void UpdateExpiredCalls()
    {
        var currentTime = AdminManager.Now;

        // Retrieve calls with no assignments whose time has expired
        var callsWithNoAssignments = _dal.Call.ReadAll()
            .Where(call => call.MaxEndCallTime <= currentTime && CallManager.CallStatus(call) != BO.Status.closed && !_dal.Assignment.ReadAll().Any(a => a.CallId == call.Id));

        // Retrieve calls with existing assignments but no actual end time
        var callsThatHaveAssignments = _dal.Call.ReadAll()
            .Where(call => call.MaxEndCallTime <= currentTime && CallManager.CallStatus(call) != BO.Status.closed && _dal.Assignment.ReadAll().Any(a => a.CallId == call.Id && a.FinishTimeTreatment == null));

        // Handle calls with no assignments
        foreach (var call in callsWithNoAssignments)
        {
            _dal.Assignment.Create(new Assignment
            {
                Id = 0,
                CallId = call.Id,
                VolunteerId = 0,
                EntryTimeTreatment = currentTime,
                FinishTimeTreatment = currentTime,
                EndTypeAssignment = DO.EndTypeAssignment.ExpiredCancellation
            });
        }

        // Handle calls with existing assignments
        foreach (var call in callsThatHaveAssignments)
        {
            var tmpAssignment = _dal.Assignment.ReadAll().First(a => a.CallId == call.Id && a.FinishTimeTreatment == null);
            _dal.Assignment.Update(new Assignment
            {
                Id = tmpAssignment.Id,
                CallId = tmpAssignment.CallId,
                VolunteerId = tmpAssignment.VolunteerId,
               EntryTimeTreatment = tmpAssignment.EntryTimeTreatment,
                FinishTimeTreatment = currentTime,
                EndTypeAssignment = DO.EndTypeAssignment.ExpiredCancellation
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
        if (assignment.FinishTimeTreatment != null)
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
        bool isAuthorized = (assignment.VolunteerId == id || _dal.Volunteer.Read(id)!.Role == DO.RoleEnum.manager);
        if (!isAuthorized)
        {
            throw new BO.BlUnauthorizedActionException("The requester does not have permission to cancel this assignment");
        }

        // Check if the assignment is still active
        if (assignment.FinishTimeTreatment != null)
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
            CompleteTreatmentTimeSpan = call.MaxEndCallTime.HasValue ? call.MaxEndCallTime.Value - DateTime.Now : null,
            LastVolunteerName = null,
             EndCallTimeSpan= null,
            Status = BO.Status.open,
            AssignSum = 0
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

    private static DO.Call BoToDo(BO.Call boCall)
    {
        return new DO.Call(
            Id: boCall.Id,
            CallAddress: boCall.CallAddress,
            Latitude:boCall.Latitude,
            Longitude:boCall.Longitude,
           StartCallTime : boCall.StartCallTime,
            CallDescription: boCall.CallDescription,
            MaxEndCallTime: boCall.MaxEndCallTime,
            CallType: (DO.CallType)boCall.CallType
 
        );
    }
    private static List<CallAssignInList> MapAssignments(List<DO.Assignment> assignments)
    {
        return assignments.Select(a => new CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            VolunteerName = _dal.Volunteer.Read(a.VolunteerId).FullName ?? "",
            EntryCallTime = a.EntryTimeTreatment,
            EndCallTime = a.FinishTimeTreatment,
            FinishType = (FinishType?)a.EndTypeAssignment
        }).ToList();
    }
    public static BO.Call ConvertToBo(DO.Call doCall)
    {
        var assignments = _dal.Assignment.ReadAll()
            .Where(a => a.CallId == doCall.Id)
            .ToList();

        return new BO.Call
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.CallType,
            CallDescription = doCall.CallDescription,
            CallAddress = doCall.CallAddress,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            StartCallTime = doCall.StartCallTime,
            MaxEndCallTime = doCall.MaxEndCallTime,
            CallAssignList = MapAssignments(assignments)
        };
    }
    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        bool callUpdated = false;

        var list = _dal.Call.ReadAll().ToList();
        foreach (var doCall in list)
        {
            var boCall = ConvertToBo(doCall); // המרת DO ל־BO
            bool updated = false;

            // תנאי 1: הדדליין עבר, הקריאה לא הושלמה ולא פג תוקף
            if (boCall.MaxEndCallTime is DateTime deadline &&
                deadline < newClock &&
                boCall.Status != BO.Status.closed &&
                boCall.Status != BO.Status.expired)
            {
                boCall.Status = BO.Status.expired;
                updated = true;
            }

            // תנאי 2: אין הקצאות, עברו 3 ימים מאז פתיחת הקריאה, סטטוס עדיין פתוח
            if ((boCall.CallAssignList == null || boCall.CallAssignList.Count == 0) &&
                (newClock - boCall.StartCallTime).TotalDays >= 3 &&
                boCall.Status == BO.Status.open)
            {
                boCall.Status = BO.Status.open;
                updated = true;
            }

            if (updated)
            {
                var updatedDo = BoToDo(boCall);
                _dal.Call.Update(updatedDo);
                Observers.NotifyItemUpdated(boCall.Id);
                callUpdated = true;
                Observers.NotifyItemUpdated(boCall.Id); //stage 5
            }
        }

        bool yearChanged = oldClock.Year != newClock.Year;
        if (yearChanged || callUpdated)
            Observers.NotifyListUpdated();
    }
    internal static void SimulateCourseRegistrationAndGrade()
    {
        throw new NotImplementedException();
    }
}
