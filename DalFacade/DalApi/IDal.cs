namespace DalApi;

public interface IDal
{
    IAssignment Assignment { get; }
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IConfig Config { get; }
    void ResetDB();
}