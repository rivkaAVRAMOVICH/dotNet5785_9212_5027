namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
        {
            throw new DalAlreadyExistsException($"Student with ID={item.Id} already exists");

        }
        else
        {
            DataSource.Volunteers.Add(item);
        }
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Volunteer item = Read(id);
        if (item == null)
        {
            throw new DalDeletionImpossible($"Volunteer with ID={id} does not exist and cannot be deleted.");
        }

        else
        {
            DataSource.Volunteers.Remove(item);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) //stage 2
      => filter == null
          ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        if (Read(item.Id) == null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist and cannot be updated.");
        }
        else
        {
            Delete(item.Id);
            DataSource.Volunteers.Add(item);
        }
    }
}
