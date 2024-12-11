namespace Dal;
using DalApi;
using DO;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int nextCallId = Config.NextCallNum;
        Call newItem = item with { Id = nextCallId };
        DataSource.Calls.Add(newItem);
    }
    
    public void Delete(int id)
    {
        Call item = Read(id);
        if (item == null)
        {
            throw new Exception($"Call with ID={id} does not exist and cannot be deleted.");
        }

        else
        {
            DataSource.Calls.Remove(item);
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(v => v.Id == id);

    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        if (Read(item.Id) == null)
        {
            throw new Exception($"Call with ID={item.Id} does not exist and cannot be updated.");
        }
        else
        {
            Delete(item.Id);
            DataSource.Calls.Add(item);
        }
    }

}