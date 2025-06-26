using BlApi;
using Helpers;
using System;
using System.Xml.Linq;
namespace BlImplementation;
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    public BO.Role EnteredSystem(int userId, string password)
    {
        DO.Volunteer tmpVolunteer;
        try
        {
            lock (AdminManager.BlMutex) //stage 7
               tmpVolunteer = _dal.Volunteer.ReadAll().Where(item => item.Id == userId).FirstOrDefault();

            // Check name
            if (tmpVolunteer == null)
            {
                throw new BO.BlDoesNotExistException($"Volunteer with id={userId} not found");
            }

            // Check password....  
            if (!Helpers.VolunteerManager.VerifyPassword(password, tmpVolunteer.Password))
            {
                throw new BO.BlUnauthorizedActionException("Incorrect password.");
            }

            return (BO.Role)tmpVolunteer.Role; ;
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException($"Login failed: {ex.Message}");
        }
    }
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? active = null, BO.CallType? sortBy = null)
    {
        IEnumerable<DO.Volunteer> volunteers;
        try
        {
            lock (AdminManager.BlMutex) //stage 7
                volunteers = _dal.Volunteer.ReadAll().ToList();
            //sort by active volunteer or not
            if (active.HasValue)
            {
                volunteers = volunteers.Where(v => v.Active == active.Value).ToList();
            }
            var volunteerList = Helpers.VolunteerManager.ConvertVolunteersToBO(volunteers).ToList(); //help function for convert to BO

            if (sortBy != null)
            {
                var sortByName = sortBy.ToString();

                // check property : Sort by property
                if (typeof(BO.VolunteerInList).GetProperty(sortByName) != null)
                {
                    volunteerList = volunteerList.OrderBy(v => typeof(BO.VolunteerInList).GetProperty(sortByName)?.GetValue(v)).ToList();
                }
                else
                {
                    throw new BO.BlUnauthorizedActionException($"Invalid sort field: {sortByName}");
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
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        DO.Volunteer tmpVolunteer;
        lock (AdminManager.BlMutex) //stage 7
            tmpVolunteer = _dal.Volunteer.Read(id);
        //cheeck id
        if (tmpVolunteer == null)
        {
            throw new BO.BlDoesNotExistException($"Volunteer whith ID : {id} does not exist.");
        }

        //new BO.Volunteer
        return Helpers.VolunteerManager.addNewVolunteerWithCall(tmpVolunteer);
    }
    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
        DO.Volunteer RequesterUpdate = _dal.Volunteer.Read(id);
        DO.Volunteer PreviousVolunteer= _dal.Volunteer.Read(volunteer.Id);
            if (Helpers.VolunteerManager.IntegrityChecker(volunteer))// help function to check if it can update
            {
                // Check if the role was changed
                bool roleChanged = PreviousVolunteer.Role != null && PreviousVolunteer.Role != (DO.RoleEnum)volunteer.Role;
                bool emailChanged = PreviousVolunteer.Email != volunteer.Email;
                bool phoneChanged = PreviousVolunteer.PhoneNumber != volunteer.Phone;
                bool addressChanged = PreviousVolunteer.FullAddress != volunteer.Address;
                //Ensure only volunteers or managers can update
                if (RequesterUpdate.Role != DO.RoleEnum.manager && roleChanged)
                {
                    throw new BO.InvalidException("Only the manager can update this information.");
                }
                if (RequesterUpdate.Id != volunteer.Id && RequesterUpdate.Role != DO.RoleEnum.manager && (emailChanged|| phoneChanged || addressChanged))
                {
                    throw new BO.InvalidException("Only the manager can update this information.");
                }
                //new object(to update) : DO.Volunteer
                DO.Volunteer finalVolunteer = Helpers.VolunteerManager.ConvertVolunteersToDo(volunteer); // help function to add new volunteer

                _dal.Volunteer.Update(finalVolunteer);//update the data of volunteers
               VolunteerManager.Observers.NotifyItemUpdated(finalVolunteer.Id);  //stage 5
               VolunteerManager.Observers.NotifyListUpdated();  //stage 5
                if (addressChanged)
                {
                    _ = VolunteerManager.UpdateCoordinatesForVolunteerAsync(finalVolunteer);
                }
            }
           
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException("Failed to update volunteer in the data layer.", ex);
        }
    }
    public void DeletingVolunteer(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        var volunteer = GetVolunteerDetails(id);

        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"No volunteer found with ID: {id}");

        // check id it's possible to delete

        if (volunteer.CallInVolunteerHandle != null)
            throw new BO.BlDeletionImpossible("Cannot delete a volunteer who is currently handling a call.");

        if (volunteer.SumHandledCalls > 0 || volunteer.SumCanceledCalls > 0 || volunteer.SumChosenExpiredCalls > 0)
            throw new BO.BlDeletionImpossible("Cannot delete a volunteer who has a history of handling calls.");

        try
        {
            // volunteer removed from data
            lock (AdminManager.BlMutex) //stage 7
                _dal.Volunteer.Delete(id);
           VolunteerManager.Observers.NotifyListUpdated();  //stage 5  
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // exceptions
            throw new BO.BlAlreadyExistException($"Error while deleting volunteer with ID: {id}. Details: {ex.Message}");
        }

    }
    public void AddingVolunteer(BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try
        {
            var myVolunteer = _dal.Volunteer.Read(volunteer.Id);

            if (Helpers.VolunteerManager.IntegrityChecker(volunteer)) // בדיקת תקינות
            {
                DO.Volunteer finalVolunteer = Helpers.VolunteerManager.ConvertVolunteersToDo(volunteer);

                _dal.Volunteer.Create(finalVolunteer); // יצירה בדאטה

                VolunteerManager.Observers.NotifyListUpdated(); // עדכון ממשק

                // 🆕 שלב 7: חישוב קואורדינטות בצורה אסינכרונית ברקע
                _ = VolunteerManager.UpdateCoordinatesForVolunteerAsync(finalVolunteer);
            }
        }
        catch (BO.BlFormatException ex)
        {
            throw new BO.BlFormatException("Invalid format in volunteer details to update");
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistException($"A volunteer with ID {volunteer.Id} already exists in the database", ex);
        }
        catch (BO.BlUnexpectedSystemException ex)
        {
            throw new BO.BlUnexpectedSystemException("An unexpected error occurred while adding the volunteer", ex);
        }
    }


}