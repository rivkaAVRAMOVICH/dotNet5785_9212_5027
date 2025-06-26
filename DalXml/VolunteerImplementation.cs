namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Read(item.Id) != null)
        {
            throw new DalAlreadyExistsException($"Student with ID={item.Id} already exists");

        }
        else
        {
            Volunteers.Add(item);
        }
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement? volunteer = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == id)
            ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteer.Remove();

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return Volunteers.FirstOrDefault(v => v.Id == id);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return Volunteers.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (filter == null)
        {
            //return Volunteers.Select(item => item);
            return Volunteers;
        }
        else
        {
            return Volunteers.Where(filter);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        List<Volunteer> Volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        if (Volunteers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist and cannot be updated.");
        Volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(Volunteers, Config.s_volunteers_xml);
    }
}