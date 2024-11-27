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
            throw new Exception($"Assignment with ID={id} does not exist and cannot be deleted.");
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

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        if (Read(item.Id) == null)
        {
            throw new Exception($"Assignment with ID={item.Id} does not exist and cannot be updated.");
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
