using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



///interface IVolunteer
namespace BlApi
{
    public interface IVolunteer: IObservable
    {
        public BO.Role EnteredSystem(int userId, string password);
        public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? active=null, BO.CallType? sortBy = null);
        public BO.Volunteer GetVolunteerDetails(int id);
        public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer);
        public void DeletingVolunteer(int id);
        public void AddingVolunteer(BO.Volunteer volunteer);
    }
}
