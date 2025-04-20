namespace DalTest;

using Dal;
using DalApi;
using DO;
using System.Net;
using static DO.Enums;

public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();
    private static void createCalls()
    {
        string[]addresses=
{
    "Dizengoff Street 1",
   "Rothschild Boulevard 45",
    "Ibn Gabirol Street 99",
    "King George Street 76",
    "Allenby Street 100",
    "Herzl Street 22",
    "Ben Yehuda Street 67",
    "Bialik Street 12",
    "Frishman Street 35",
    "Bugrashov Street 40",
    "Arlozorov Street 5",
  "Hashmonaim Street 80",
    "Yehuda Halevi Street 90",
    "HaYarkon Street 15",
    "Allenby Street 120",
};
        for (int i = 0; i < 10; i++)
        {
            int id =0;
            Enums.CallTypeEnum callType = 0;
            string address = addresses[s_rand.Next(addresses.Length)];
            double latitude = Math.Round(s_rand.NextDouble() * 180 - 90, 6); // Random latitude
            double longitude = Math.Round(s_rand.NextDouble() * 360 - 180, 6); // Random longitude
            DateTime openHour = s_dal.Config.Clock.AddHours(-s_rand.Next(1, 48)); // Randomly in the last 2 days
            DateTime closeHour = openHour.AddHours(s_rand.Next(1, 5)); // Duration between 1 to 5 hours
            string? description = s_rand.Next(2) == 0 ? "Urgent response required" : null;

            var newCall = new Call(
                id,
                address,
                longitude,
                latitude,
                openHour,
                callType,
                description,
                closeHour
                
            );

            s_dal.Call.Create(newCall);
        }
    }
    private static void createAssignments()
    {
        var callIds = s_dal.Call.ReadAll()?.Select(c => c.Id).ToList(); // Retrieve and map call IDs
        var volunteerIds = s_dal.Volunteer.ReadAll()?.Select(v => v.Id).ToList(); // Retrieve and map volunteer IDs

        for (int i = 0; i < 10; i++)
        {
            int id = 0;//ConfigImplementation.NextAssignmentId
            int callId = callIds[s_rand.Next(callIds.Count)];
            int volunteerId = volunteerIds[s_rand.Next(volunteerIds.Count)];
            DateTime assignmentStart = s_dal.Config.Clock.AddMinutes(-s_rand.Next(1, 60)); // Random start within the last hour
            DateTime? assignmentEnd = s_rand.Next(2) == 0 ? (DateTime?)null : assignmentStart.AddMinutes(s_rand.Next(10, 120));
            Enums.finishTreatmentTypeEnum? endKind = assignmentEnd.HasValue
                ? (Enums.finishTreatmentTypeEnum?)s_rand.Next(Enum.GetValues(typeof(Enums.finishTreatmentTypeEnum)).Length)
                : null;

            var newAssignment = new Assignment(
                id,
                callId,
                volunteerId,
                assignmentStart,
                assignmentEnd,
                endKind
            );

            s_dal.Assignment.Create(newAssignment);
        }
    }
    private static void createVolunteers()
    {
        string[] volunteersNames =
       { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof" };
        string[] prefixes = { "050", "052", "054", "058", "055", "053", "057" };
        int id;
        string phoneNumber;
        string email;
        string? password;
        RoleEnum role;
        DistanceTypeEnum distanceType;
        int maxDistance;
        int i = 0;
        var locations = new (string Address, double? Latitude, double? Longitude)[]
{
    ("Dizengoff Street 1", 32.0853, 34.7818),
    ("Rothschild Boulevard 45", 32.0860, 34.7825),
    ("Ibn Gabirol Street 99", 32.0845, 34.7801),
    ("King George Street 76", 32.0872, 34.7799),
    ("Allenby Street 100", 32.0858, 34.7832),
    ("Herzl Street 22", 32.0837, 34.7814),
    ("Ben Yehuda Street 67", 32.0883, 34.7827),
    ("Bialik Street 12", 32.0865, 34.7841),
    ("Frishman Street 35", 32.0849, 34.7808),
    ("Bugrashov Street 40", 32.0877, 34.7835),
    ("Arlozorov Street 5", 32.0851, 34.7802),
    ("Hashmonaim Street 80", 32.0842, 34.7819),
    ("Yehuda Halevi Street 90", 32.0869, 34.7830),
    ("HaYarkon Street 15", 32.0874, 34.7823),
    ("Allenby Street 120", 32.0856, 34.7812)
};
        foreach (var name in volunteersNames)
        {
          
            do
            {
              id = s_rand.Next(200000000, 400000000);
               
            }
            while (s_dal!.Volunteer.Read(id) != null);
            phoneNumber = prefixes[s_rand.Next(0, prefixes.Length)] + s_rand.Next(1000000, 10000000);
            email = phoneNumber + "@gmail.com";
            password = $"{new string(name.Where(char.IsLetter).Take(4).ToArray())}{id.ToString().Substring(0, 4)}{"!@#$%^&*()"[s_rand.Next(10)]}";
            maxDistance = s_rand.Next(20, 100);
            role = Enums.RoleEnum.volunteer;
            distanceType = Enums.DistanceTypeEnum.aerialDistance;
            i++;
            s_dal!.Volunteer.Create(new Volunteer(id, name, phoneNumber, email, false, role, distanceType, password,locations[i].Address, locations[i].Latitude, locations[i].Longitude, maxDistance));
        }

    }
    public static void Do()
    {

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal = DalApi.Factory.Get;
        //...
        s_dal.ResetDB();
        Console.WriteLine("Initializing volunteer list ...");
        createVolunteers();
        Console.WriteLine("Initializing Call list ...");
        createCalls();
        Console.WriteLine("Initializing Assignments list ...");
        createAssignments();
        //...
    }

}
