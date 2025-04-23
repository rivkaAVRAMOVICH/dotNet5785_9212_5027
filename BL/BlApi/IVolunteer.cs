using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



///interface IVolunteer
namespace BlApi
{
    public interface IVolunteer
    {
        public BO.Role EnteredSystem(string userName, string password);
        public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? active,BO.HandleCallType? sortBy = null);
        public BO.Volunteer GetVolunteerDetails(int id);
        public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer);
        public void DeletingVolunteer(int id);
        public void AddingVolunteer(BO.Volunteer volunteer);
    }
}
