using BlApi;
using Helpers;
using System;
namespace BlImplementation;
    internal class VolunteerImplementation : IVolunteer
    {
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public BO.Role EnteredSystem(string userName, string password) {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll()
                .FirstOrDefault(v => v.FullName == userName && v.Password == password);
            if (volunteer == null)
                throw new BO.EntityNotFoundException("שם משתמש או סיסמה אינם נכונים");

            return (BO.Role)volunteer.Role;
        }
        catch (DO.EntityNotFoundException ex)
        {
            throw new BO.EntityNotFoundException("שגיאה בגישה לנתונים: " + ex.Message, ex);
        }
    }
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? active, BO.HandleCallType? sortBy=null) {
        
        try
        {
            var volunteers = _dal.Volunteer.ReadAll();
            //sort by active volunteer or not
            if (active.HasValue)
            {
                volunteers = volunteers.Where(v => v.Active == active.Value).ToList();
            }
            var volunteerList = Helpers.VolunteerManager.ConvertVolunteersToBO(volunteers).ToList(); //help function for convert to BO

            if (sortBy != null)
            {
                var sortsortByName = sortBy.ToString();

                // check property : Sort by property
                if (typeof(BO.VolunteerInList).GetProperty(sortsortByName) != null)
                {
                    volunteerList = volunteerList
                        .OrderBy(v => typeof(BO.VolunteerInList).GetProperty(sortsortByName)?.GetValue(v))
                        .ToList();
                }
                else
                {
                    throw new BO.BlUnauthorizedActionException($"Invalid sort field: {sortsortByName}");
                }
            }
            else
            {
                // sort by id
                volunteerList = volunteerList.OrderBy(v => v.Id).ToList();
            }

            // full volunteer list
            return volunteerList;
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            // error handling
            throw new BO.BlUnexpectedSystemException($"Error while fetching the volunteer list: {ex.Message}", ex);
        }
    }
    public BO.Volunteer GetVolunteerDetails(int id){
       
        var tmpVolunteer = _dal.Volunteer.Read(id);
        //cheeck id
        if (tmpVolunteer == null)
        {
            throw new BO.BlDoesNotExistException($"Volunteer whith ID : {id} does not exist.");
        }

        //new BO.Volunteer
        var final = Helpers.VolunteerManager.addNewVolunteerWithCall(tmpVolunteer);

        //check call and get
        var callInProgress = _dal.Call.ReadAll()
            .FirstOrDefault(c => c.Id == id && CallManager.CallStatus(c) == BO.Status.InProgress);
        if (callInProgress != null && final != null && final.Latitude.HasValue && final.Longitude.HasValue)
        {
            var finalCall = Helpers.VolunteerManager.addNewCall(callInProgress, final); // help function to add new call to the volunteer
            return finalCall;
        }
        else
            throw new BO.BlUnauthorizedActionException("Error to return the volunteer");

    }
    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        var PreviousVolunteer = _dal.Volunteer.Read(id);
        var myStatus = (BO.Role)PreviousVolunteer?.Role;
        try
        {

            if (Helpers.VolunteerManager.checkedToUpdate(volunteer, myStatus))// help function to check if it can update
            {
                //Check what was changed
                //bool roleChanged = PreviousVolunteer.Role.HasValue && PreviousVolunteer.Role != (DO.Role)volunteer.Role;
                //bool emailChanged = PreviousVolunteer.Email != volunteer.Email;
                //bool phoneChanged = PreviousVolunteer.PhoneNumber != volunteer.PhoneNumber;
                //bool addressChanged = PreviousVolunteer.Address != volunteer.Address;

                //new object (to update) : DO.Volunteer
                DO.Volunteer finalVolunteer = Helpers.VolunteerManager.addNewVolunteer(PreviousVolunteer, volunteer); // help function to add new volunteer


                _dal.Volunteer.Update(finalVolunteer);//update the data of volunteers
            }
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException("Failed to update volunteer in the data layer.", ex);
        }
    }
    public void DeletingVolunteer(int id)
    {
        var volunteer = _dal.Volunteer.Read(id);

        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"No volunteer found with ID: {id}");

        // check id it's possible to delete
        if (volunteer.callInProgress != null)
            throw new BO.BlDeletionImpossible("Cannot delete a volunteer who is currently handling a call.");

        if (volunteer.SumOfCalls > 0 || volunteer.SumOfCanceledCalls > 0 || volunteer.SumOfExpiredCalls > 0)
            throw new BO.BlDeletionImpossible("Cannot delete a volunteer who has a history of handling calls.");

        try
        {
            // volunteer removed from data
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // exceptions
            throw new BO.BlAlreadyExistException($"Error while deleting volunteer with ID: {id}. Details: {ex.Message}");
        }

    }
    public void AddingVolunteer(BO.Volunteer volunteer){
        throw new NotImplementedException();
    }

}

