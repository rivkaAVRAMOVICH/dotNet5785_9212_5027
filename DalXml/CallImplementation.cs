namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

internal class CallImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int nextCallId = Config.NextCallId;
        Call newItem = item with { Id = nextCallId };
        Calls.Add(newItem);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        XElement? call = callsRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == id)
            ?? throw new DO.DalDoesNotExistException($"Call with ID={id} does not exist");

        call.Remove();

        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(v => v.Id == id);

    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return Calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (filter == null)
        {
            //return Calls.Select(item => item);
            return Calls;
        }
        else
        {
            return Calls.Where(filter);
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist and cannot be updated.");
        Calls.Add(item);
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }
}
