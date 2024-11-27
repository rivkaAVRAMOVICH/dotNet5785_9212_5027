namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) != null)
        {
            throw new Exception($"Volunteer with ID={item.Id} already exists. Cannot create a new volunteer with the same ID.");
        }
        else
        {
            DataSource.Volunteers.Add(item);
        }
    }


    public void Delete(int id)
    {
        Volunteer item = Read(id);
        if (item == null)
        {
            throw new Exception($"Volunteer with ID={id} does not exist and cannot be deleted.");
        }

        else
        {
            DataSource.Volunteers.Remove(item);
        }
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();

    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        if (Read(item.Id) == null)
        {
            throw new Exception($"Volunteer with ID={item.Id} does not exist and cannot be updated.");
        }
        else
        {
            Delete(item.Id);
            DataSource.Volunteers.Add(item);
        }
    }
}
