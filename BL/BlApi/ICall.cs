using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface ICall
    {
        int[] RequestCallsQuantities();
        IEnumerable<BO.CallInList> GetCallsList(BO.CallType? filterField, object? filterValue, BO.CallType? sortBy);
        BO.Call GetCallsDetails(int id);
        void UpdateCallDetails(BO.Call call);
        void DeletingCall(int id);
        void AddingCall(BO.Call call);
        IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.CallType? sortBy);
        IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.CallType? sortBy);
        void ReportCallCompletion(int volunteerId, int assignmentId);
        void CancelCallHandling(int requesterId, int assignmentId);
        void AssignCallToVolunteer(int volunteerId, int callId);
    }
}
