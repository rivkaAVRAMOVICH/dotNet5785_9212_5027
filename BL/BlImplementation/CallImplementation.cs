namespace BlImplementation;
using BlApi;
using Helpers;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Retrieves the number of calls grouped by their status
    /// </summary>
    /// <returns>
    /// An array where each index represents a call status and its value indicates the count of calls in that status
    /// The mapping of indices is as follows:
    /// 0: open, 1:inProgress, 2:closed, 3:expired, 4:openAtRisk, 5:inProgressAtRisk
    /// </returns>
    public int[] RequestCallsQuantities()
    {
        int[] final = new int[6];
        var callsByStatus = _dal.Call.ReadAll().GroupBy(call => CallManager.CallStatus(call));

        // Group calls by their status and populate the final array with the count of calls in each status.
        foreach (var callGroup in callsByStatus)
        {
            int statusIndex = (int)callGroup.Key;
            if (statusIndex >= 0 && statusIndex < final.Length)
                final[statusIndex] = callGroup.Count();
        }
        return final;
    }

    /// <summary>
    /// Retrieves a list of calls filtered and sorted according to the given parameters
    /// </summary>
    /// <param name="filterField">Optional filter field to narrow down the results</param>
    /// <param name="filterValue">Optional value to match against the filter field</param>
    /// <param name="sortField">Optional field to sort the results</param>
    /// <returns>A collection of BO.CallInList objects</returns>
    public IEnumerable<BO.CallInList> GetCallsList(BO.CallType? filterField, object? filterValue, BO.CallType? sortBy)
    {
        var callList = CallManager.ConvertCallsToBO(_dal.Call.ReadAll());

        // Apply filtering if necessary
        if (filterField != null && filterValue != null)
        {
            // Retrieve the value of the specified filter field and compare it to the filter value.
            callList = callList.Where(item =>
            {
                var fieldValue = CallManager.GetFieldValue(item, filterField);
                return fieldValue != null && fieldValue.Equals(filterValue);
            });
        }

        // Apply sorting if a sort field is provided
        if (sortBy != null)
        {
            // Sort the list based on the provided sort field.
            callList = callList.OrderBy(call => CallManager.GetFieldValue(call, sortBy));
        }
        else
        {
            callList = callList.OrderBy(call => call.Id);
        }

        // Handle duplicate call IDs by keeping only the most recent entry.
        if (callList.Any())
        {
            callList = callList.GroupBy(a => a.CallId)
                .Select(g => g.OrderByDescending(a => a.TimeCallMade).FirstOrDefault());
        }

        return callList;
    }

    /// <summary>
    /// Retrieves detailed information for a specific call by its ID
    /// </summary>
    /// <param name="id">The ID of the call to retrieve</param>
    /// <returns>A BO.Call object containing detailed information about the call</returns>
    /// <exception cref="Exception">Thrown when the call ID does not exist</exception>
    public BO.Call GetCallsDetails(int id)
    {
        try
        {
            // Retrieve call data from the data layer
            var tmpCall = _dal.Call.Read(id);

            // Build a BO.Call object with assignments related to the call
            var boCall = new BO.Call
            {
                Id = tmpCall.Id,
                CallType = (BO.CallType)tmpCall.CallType,
                CallDescription = tmpCall.CallDescription,
                CallAddress = tmpCall.CallAddress,
                Latitude = tmpCall.Latitude,
                Longitude = tmpCall.Longitude,
                StartCallTime = tmpCall.StartCallTime,
                MaxEndCallTime = tmpCall.MaxEndCallTime,
                Status = CallManager.CallStatus(tmpCall),
                CallAssignList = _dal.Assignment?.ReadAll().Where(item => item.CallId == tmpCall.Id).Select(a => new BO.CallAssignInList
                {
                    VolunteerId = a.VolunteerId,
                    VolunteerName = _dal.Volunteer.Read(a.VolunteerId)!.Name,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    FinishType = (BO.FinishType)a.FinishType
                }).ToList()
            };

            return boCall;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={id} does not exist.", ex);
        }
    }

    /// <summary>
    /// Updates an existing call in the system
    /// </summary>
    /// <param name="call">A BO.Call object containing the updated call details</param>
    /// <exception cref="BO.BlValidationException">Thrown when the call details fail validation, such as when MaxTime is earlier than TimeCallMade</exception>
    /// <exception cref="BO.BlNullPropertyException">Thrown when a required property of the call, such as Address, is null or empty</exception>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the call with the specified ID does not exist in the system</exception>
    /// <exception cref="BO.BlUnexpectedSystemException">Thrown when an unexpected system error occurs during the update process</exception>
    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            // Validate the call's format and logic
            CallManager.ValidateCall(call);

            // Convert BO.Call to DO.Call
            var newCall = new DO.Call
            {
                Id = call.Id,
                CallType = (DO.CallType)call.CallType,
                VerbalDescription = call.CallDescription,
                CallAddress = call.CallAddress,
                Latitude = CallManager.GetLatitudLongitute(call.CallAddress).Latitude, // Get latitude for the address
                Longitude = CallManager.GetLatitudLongitute(call.CallAddress).Longitude, // Get longitude for the address
                openTime = call.StartCallTime,
                MaxEndCallTime = call.MaxEndCallTime
            };

            // Attempt to update the call in the data layer
            _dal.Call.Update(newCall);
        }

        catch (BO.BlValidationException ex)
        {
            throw new BO.BlValidationException("Max time must be later than the time the call was made", ex);
        }

        catch (BO.BlNullPropertyException ex)
        {
            throw new BO.BlNullPropertyException("Address cannot be null or empty");
        }

        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"The call with ID={call.Id} was not found", ex);
        }

        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException("An unexpected error occurred while updating the call", ex);
        }

    }

    /// <summary>
    /// Removes a call and its related assignments if the call is open or openAtRisk
    /// </summary>
    /// <param name="id">The ID of the call to remove</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified call ID does not exist in the system</exception>
    public void DeletingCall(int id)
    {
        try
        {
            var tmpCall = _dal.Call.Read(id);
            var tmpAssignments = _dal.Assignment.ReadAll().Where(item => item.CallId == id);
            var tmpStatus = CallManager.CallStatus(tmpCall!);

            // Only delete the call if its status is 'open' or 'openAtRisk'.
            foreach (var item in tmpAssignments)
            {
                if (tmpStatus == BO.Status.open || tmpStatus == BO.Status.openAtRisk)
                    _dal.Call.Delete(id);
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={id} was not found", ex);
        }
    }

    /// <summary>
    /// Adds a new call to the system
    /// </summary>
    /// <param name="call">A BO.Call object containing the call details</param>
    /// <exception cref="BO.BlValidationException">Thrown when the call details fail validation, such as when MaxTime is earlier than TimeCallMade</exception>
    /// <exception cref="BO.BlNullPropertyException">Thrown when a required property of the call, such as Address, is null or empty</exception>
    /// <exception cref="BO.BlAlreadyExistException">Thrown when a call with the specified ID already exists in the system</exception>
    /// <exception cref="BO.BlUnexpectedSystemException">Thrown when an unexpected system error occurs during the addition process</exception>
    public void AddingCall(BO.Call call)
    {
        try
        {
            // Validate the call's format and logic
            CallManager.ValidateCall(call);

            // Create a DO.Call object from the given data
            var finalCall = new DO.Call
            {
                Id = call.Id,
                CallType = (DO.CallType)call.CallType,
                VerbalDescription = call.CallDescription,
                CallAddress = call.CallAddress,
                Latitude = CallManager.GetLatitudLongitute(call.CallAddress).Latitude, // Get latitude for the address
                Longitude = CallManager.GetLatitudLongitute(call.CallAddress).Longitude, // Get longitude for the address
                openTime = call.StartCallTime,
                MaxEndCallTime = call.MaxEndCallTime
            };

            // Attempt to add the new call in the data layer
            _dal.Call.Create(finalCall);
        }

        catch (BO.BlValidationException ex)
        {
            throw new BO.BlValidationException("Max time must be later than the time the call was made", ex);
        }
        catch (BO.BlNullPropertyException ex)
        {
            throw new BO.BlNullPropertyException("Address cannot be null or empty");
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException($"Call with ID={call.Id} already exists", ex);
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException("An unexpected error occurred while adding the call", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of closed calls for a specific volunteer
    /// Filters calls by type and optionally sorts them by a specified field
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <param name="callType">Optional filter by type of call</param>
    /// <param name="sortField">Optional sorting field (e.g., CallType, FinishType)</param>
    /// <returns>A collection of closed calls formatted as BO.ClosedCallInList objects</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.CallType? sortBy)
    {
        // Fetch all closed assignments for the volunteer
        var closedVolunteerAssignments = _dal.Assignment.ReadAll()
            .Where(assign => assign.VolunteerId == id && assign.FinishType != null);

        // Fetch all calls associated with the closed assignments
        var closedVolunteerCalls = _dal.Call.ReadAll()
            .Where(call => closedVolunteerAssignments.Any(a => a.CallId == call.Id));

        // Filter calls by type if specified
        if (callType.HasValue)
        {
            closedVolunteerCalls = closedVolunteerCalls.Where(call => call.CallType == (DO.CallType)callType.Value);
        }

        // Transform data into BO.ClosedCallInList objects
        IEnumerable<BO.ClosedCallInList> finalCalls = closedVolunteerCalls.Select(call => new BO.ClosedCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            Address = call.CallAddress,
            TimeCallMade = call.StartCallTime,
            StartTime = call.StartCallTime,
            EndTime = (DateTime)closedVolunteerAssignments.FirstOrDefault(assign => assign.CallId == call.Id).EndTime,
            MaxEndCallTime = BO.FinishType.takenCareOf
        });

        // Sort calls by the specified field
        finalCalls = sortBy switch
        {
            BO.CallType => finalCalls.OrderBy(call => call.CallType),
            BO.MaxEndCallTime => finalCalls.OrderBy(call => call.MaxEndCallTime),
            _ => finalCalls.OrderBy(call => call.Id) // Default sorting by ID
        };

        return finalCalls;
    }

    /// <summary>
    /// Retrieves a list of open calls for a volunteer
    /// Filters calls by type and optionally sorts them by a specified field
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <param name="callType">Optional filter by type of call</param>
    /// <param name="sortField">Optional sorting field (e.g., CallType)</param>
    /// <returns>A collection of open calls formatted as BO.OpenCallInList objects</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified volunteer ID does not exist in the system</exception>
    /// <exception cref="BO.BlFormatException">Thrown when the volunteer's location is not available for distance calculation</exception>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.CallType? sortBy)
    {
        // Fetch all open calls
        var openCalls = _dal.Call.ReadAll()
            .Where(call => CallManager.CallStatus(call) == BO.Status.open || CallManager.CallStatus(call) == BO.Status.openAtRisk);

        // Filter calls by type if specified
        if (callType.HasValue)
        {
            openCalls = openCalls.Where(call => call.CallType == (DO.CallType)callType.Value);
        }

        DO.Volunteer volunteer;
        // Retrieve the volunteer's location
        try
        {
            volunteer = _dal.Volunteer.Read(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} wass not found", ex);
        }
        if (volunteer.Latitude == null || volunteer.Longitude == null)
        {
            throw new BO.BlFormatException("Volunteer location is not available");
        }

        double volunteerLat = volunteer.Latitude.Value;
        double volunteerLon = volunteer.Longitude.Value;

        // Transform data into BO.OpenCallInList objects with distance calculation
        IEnumerable<BO.OpenCallInList> finalCalls = openCalls.Select(call => new BO.OpenCallInList
        {
            Id = call.Id,
            CallType = (BO.CallType)call.CallType,
            Description = call.Description,
            Address = call.Address,
            TimeCallMade = call.TimeCallMade,
            MaxTime = call.MaxTime,
            Distance = CallManager.GetDistance(volunteerLat, volunteerLon, call.Latitude, call.Longitude)
        });

        // Sort calls by the specified field
        finalCalls = sortField switch
        {
            BO.CallType => finalCalls.OrderBy(call => call.CallType),
            _ => finalCalls.OrderBy(call => call.Id) // Default sorting by ID
        };

        return finalCalls;
    }

    /// <summary>
    /// Marks a call as completed by a volunteer
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified assignment does not exist in the system</exception>
    /// <exception cref="BO.BlUnauthorizedActionException">Thrown when the assignment does not belong to the specified volunteer</exception>
    /// <exception cref="BO.BlInvalidAssignmentException">Thrown when the assignment has already been completed or is no longer active</exception>
    /// <exception cref="BO.BlUnexpectedSystemException">Thrown when an unexpected system error occurs while completing the assignment</exception>
    public void ReportCallCompletion(int volunteerId, int assignmentId)
    {
        try
        {
            // Fetch the assignment data
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId);

            // Validate the assignment's eligibility to be closed
            CallManager.ValidateAssignmentToFinish(id, assignment);

            // Update the assignment with the completion details
            _dal.Assignment.Update(new DO.Assignment(0, assignment.CallId, id, assignment.StartTime, DateTime.Now, DO.FinishType.takenCareOf));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle missing assignment exception
            throw new BO.BlDoesNotExistException($"The assignment with ID={assignmentId} was not found", ex);
        }

        catch (BO.BlUnauthorizedActionException ex)
        {
            // Handle missing assignment exception
            throw new BO.BlUnauthorizedActionException($"The assignment does not belong to the specified volunteer", ex);
        }

        catch (BO.BlInvalidAssignmentException ex)
        {
            // Handle missing assignment exception
            throw new BO.BlInvalidAssignmentException($"The assignment has already been completed or is no longer active", ex);
        }

        catch (Exception ex)
        {
            // Handle general exceptions
            throw new BO.BlUnexpectedSystemException("An unexpected error occurred while completing the assignment", ex);
        }
    }

    /// <summary>
    /// Cancels an assignment for a volunteer
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <param name="assignmentId">Assignment ID</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified assignment does not exist in the system</exception>
    /// <exception cref="BO.BlUnauthorizedActionException">Thrown when the volunteer does not have permission to cancel the assignment</exception>
    /// <exception cref="BO.BlInvalidAssignmentException">Thrown when the assignment has already been completed or is no longer active</exception>
    /// <exception cref="BO.BlUnexpectedSystemException">Thrown when an unexpected system error occurs while canceling the assignment</exception>
    public void CancelCallHandling(int requesterId, int assignmentId)
    {
        try
        {
            // Fetch the assignment data
            DO.Assignment assignment = _dal.Assignment.Read(assignmentId);

            // Validate the assignment's eligibility to be canceled
            CallManager.ValidateAssignmentToCancel(id, assignment);

            // Determine the finish type based on who is canceling
            var VolunteerOrManager = (assignment.VolunteerId == id)
                ? DO.FinishType.volunteerCanceled
                : DO.FinishType.managerCanceledAssignment;

            // Update the assignment with the cancellation details
            _dal.Assignment.Update(new DO.Assignment(0, assignment.CallId, id, assignment.StartTime, ClockManager.Now, VolunteerOrManager));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle missing assignment exception
            throw new BO.BlDoesNotExistException($"The assignment with ID={assignmentId} was not found", ex);
        }
        catch (BO.BlUnauthorizedActionException ex)
        {
            // Handle unauthorized access
            throw new BO.BlUnauthorizedActionException("You do not have permission to cancel this assignment", ex);
        }
        catch (BO.BlInvalidAssignmentException ex)
        {
            // Handle invalid operation
            throw new BO.BlInvalidAssignmentException("The assignment has already been completed or is no longer active", ex);
        }
        catch (Exception ex)
        {
            // Handle general exceptions
            throw new BO.BlUnexpectedSystemException("An unexpected error occurred while canceling the assignment");
        }
    }

    /// <summary>
    /// Assigns a volunteer to handle a call
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <param name="callId">Call ID</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown when the specified volunteer or call does not exist in the system</exception>
    /// <exception cref="BO.BlInvalidAssignmentException">Thrown when the call is closed, expired, or already being handled</exception>
    /// <exception cref="BO.BlAlreadyExistException">Thrown when an assignment with the specified details already exists</exception>
    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        DO.Volunteer tmpVol;
        DO.Call tmpCall;

        try
        {
            // Fetch volunteer data
            tmpVol = _dal.Volunteer.Read(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} not found", ex);
        }

        try
        {
            // Fetch call data
            tmpCall = _dal.Call.Read(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} not found", ex);
        }

        // Validate the call status
        if (CallManager.CallStatus(tmpCall) != BO.Status.open && CallManager.CallStatus(tmpCall) != BO.Status.openAtRisk)
        {
            throw new BO.BlInvalidAssignmentException($"Call is not closed, expired or already being handled");
        }

        // Create a new assignment for the volunteer
        var tmpAssign = new DO.Assignment(0, callId, id, ClockManager.Now, null, null);
        try
        {
            _dal.Assignment.Create(tmpAssign);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException($"Assignment with ID={tmpAssign.Id} already exists", ex);
        }
    }

}
