namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

public class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        int nextCallId = Config.NextCallNum;
        Call newItem = item with { Id = nextCallId };
        DataSource.Calls.Add(newItem);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Call item = Read(id);
        if (item == null)
        {
            throw new DalDeletionImpossible($"Call with ID={id} does not exist and cannot be deleted.");
        }

        else
        {
            DataSource.Calls.Remove(item);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(v => v.Id == id);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) //stage 2
       => filter == null
           ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        if (Read(item.Id) == null)
        {
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist and cannot be updated.");
        }
        else
        {
            Delete(item.Id);
            DataSource.Calls.Add(item);
        }
    }
}