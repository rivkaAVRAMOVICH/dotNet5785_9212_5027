namespace Dal;
using DalApi;
using DO;

public class AssignmentImplementation : IAssignment
{
    public void Delete(int id)
    {
        Assignment item = Read(id);
        if (item == null)
        {
            throw new DalDeletionImpossible($"Assignment with ID={id} does not exist and cannot be deleted.");
        }

        else
        {
            DataSource.Assignments.Remove(item);
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(v => v.Id == id);

    }
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
       => filter == null
           ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);


    public void Update(Assignment item)
    {
        if (Read(item.Id) == null)
        {
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist and cannot be updated.");
        }
        else
        {
            Delete(item.Id);
            DataSource.Assignments.Add(item);
        }

    }


    public void Create(Assignment item)
    {
        int nextAssignmentId = Config.NextAssignmentId;
        Assignment news = item with { Id = nextAssignmentId };
        DataSource.Assignments.Add(news);
    }
}
