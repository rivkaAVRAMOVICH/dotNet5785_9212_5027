namespace BlTest;
using BO;
internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        PrintMainMenu();
        int choice = int.Parse(Console.ReadLine()!);

        while (choice > 0)
        {
            try
            {
                switch (choice)
                {
                    case 0:
                        //exit
                        Console.WriteLine("Exit");
                        break;

                    case 1:
                        //volunteer sub menu
                        VolunteerSubMenu();
                        break;

                    case 2:
                        //call sub menu
                        CallSubMenu();
                        break;

                    case 3:
                        //initializing all the data
                        InitializeDB();
                        break;

                    case 4:
                        //view all the data
                        AllVolunteers();
                        AllCalls();
                        break;

                    case 5:
                        //configuration sub menu
                        AdminSubMenu();
                        break;

                    case 6:
                        //reset everything
                        ResetDB();
                        break;

                }
            }
            catch (Exception ex) { Console.WriteLine($"Error:{ex}"); }
            PrintMainMenu();
            choice = int.Parse(Console.ReadLine()!);
        }
    }

    /// <summary>
    /// Displays the main menu.
    /// </summary>
    private static void PrintMainMenu()
    {
        Console.Write(@"Main Menu:
1- Volunteer Menu
2- Call Menu
3- Initialization DataBase
4- Display All Data
5- Admin Menu
6- Reset All Data and Configuration
0- exit
Enter your choice: 
");
    }

    /// <summary>
    /// Displays a submenu for a specified entity.
    /// </summary>
    /// <param name="entity">The name of the entity (e.g., Volunteer, Call).</param>
    private static void PrintSubMenu(string entity)
    {
        Console.Write($"{entity} Menu: ");
        Console.WriteLine($"1- Add {entity}");
        Console.WriteLine($"2- Show {entity} details");
        Console.WriteLine($"3- Show all {entity}s' details");
        Console.WriteLine($"4- Update {entity}");
        Console.WriteLine($"5- Delete {entity}");
        if (entity == "Volunteer")
        {
            Console.WriteLine($"6- {entity} login");
            Console.WriteLine($"7- Assign Call to {entity}");
        }
        if (entity == "Call")
        {
            Console.WriteLine($"6- Show all Closed {entity}s");
            Console.WriteLine($"7- Show all Open {entity}s");
            Console.WriteLine($"8- Get number of {entity}s per Status");
            Console.WriteLine($"9- Assign {entity} to Volunteer");
        }
        Console.WriteLine("0- exit");
        Console.WriteLine("Enter your choice:");
    }

    /// <summary>
    /// Displays a submenu to update call-related information.
    /// </summary>
    private static void UpdateCallSubMenu()
    {
        Console.WriteLine("What would you like to update?");
        Console.WriteLine("1- Cancel Call");
        Console.WriteLine("2- Finish Call");
        Console.WriteLine("3- General Update");
        Console.WriteLine("enter your choice:");
        int choice = int.Parse(Console.ReadLine());
        if (choice == 1)
            UpdateCancelTreatment();
        if (choice == 2)
            UpdateFinishTreatment();
        if (choice == 3)
            UpdateCall();
    }

    /// <summary>
    /// Displays and processes options for the Volunteer submenu.
    /// </summary>
    private static void VolunteerSubMenu()
    {
        PrintSubMenu("Volunteer");
        int subChoice = int.Parse(Console.ReadLine()!);

        while (subChoice > 0)
        {
            try
            {
                switch (subChoice)
                {
                    case 0:
                        //exit
                        return;
                    case 1:
                        //create
                        AddVolunteer();
                        break;
                    case 2:
                        //read
                        VolunteerDetails();
                        break;
                    case 3:
                        //read all
                        AllVolunteers();
                        break;
                    case 4:
                        //update
                        UpdateVolunteer();
                        break;
                    case 5:
                        //delete
                        DeleteVolunteer();
                        break;
                    case 6:
                        //login
                        VolunteerLogin();
                        break;
                    case 7:
                        //assign call
                        AssignCall();
                        break;

                }

            }
            catch (Exception ex) { Console.WriteLine($"Error:{ex}"); }
            Console.Write(@"
Enter your choice: ");
            PrintMainMenu();
        }

    }

    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    private static void AddVolunteer()
    {
        Console.WriteLine("Enter volunteer ID: ");
        int v1id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter volunteer name: ");
        string v1name = Console.ReadLine()!;
        Console.WriteLine(@"Enter volunteer role (0- manager, 1- volunteer): ");
        Role v1role = (Role)int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter vlunteer phone: ");
        string v1phoneNumber = Console.ReadLine()!;
        Console.WriteLine("Enter volunteer email: ");
        string v1email = Console.ReadLine()!;
        Console.WriteLine("Enter volunteer password: ");
        string v1password = Console.ReadLine()!;
        Console.WriteLine("Enter volunteer address: ");
        string v1address = Console.ReadLine()!;
        Console.WriteLine("Enter maximum distance: ");
        double v1maxDistance = double.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Enter type of distance (0- driving, 1- walking, 2- air distance: ");
        TypeOfDistance v1distanceType = (TypeOfDistance)int.Parse(Console.ReadLine()!);

        Volunteer v1 = new Volunteer
        {
            Id = v1id,
            Name = v1name,
            Role = v1role,
            Phone = v1phoneNumber,
            Email = v1email,
            Password = v1password,
            Address = v1address,
            MaxDistance = v1maxDistance,
            DistanceType = v1distanceType,
            IsActive = true
        };
        s_bl.Volunteer.AddingVolunteer(v1);
        Console.WriteLine("Volunteer Added Successfully");
    }

    /// <summary>
    /// Retrieves details of a volunteer by ID.
    /// </summary>
    private static void VolunteerDetails()
    {
        Console.WriteLine("Enter Volunteer ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var myVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            Console.WriteLine(myVolunteer != null ? myVolunteer.ToString() : "Volunteer not found.");
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }

    }

    /// <summary>
    /// Displays all volunteers in the system.
    /// </summary>
    private static void AllVolunteers()
    {
        if (s_bl!.Volunteer != null)
        {
            foreach (var isVol in s_bl.Volunteer.GetVolunteersList(null, null))
            {
                Console.WriteLine(isVol);
            }
        }
    }

    /// <summary>
    /// Updates the details of an existing volunteer in the system.
    /// </summary>
    private static void UpdateVolunteer()
    {
        Console.WriteLine("Enter Volunteer ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (s_bl.Volunteer.GetVolunteerDetails(id) != null)
            {
                Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id));

                //gets all the info of the volunteer and adds it to DataSource
                Console.WriteLine("Enter volunteer name: ");
                string v1name = Console.ReadLine()!;
                Console.WriteLine(@"Enter volunteer role (0- manager, 1- volunteer): ");
                Role v1role = (Role)int.Parse(Console.ReadLine()!);
                Console.WriteLine("Enter vlunteer phone: ");
                string v1phoneNumber = Console.ReadLine()!;
                Console.WriteLine("Enter volunteer email: ");
                string v1email = Console.ReadLine()!;
                Console.WriteLine("Enter volunteer password: ");
                string v1password = Console.ReadLine()!;
                Console.WriteLine("Enter volunteer address: ");
                string v1address = Console.ReadLine()!;
                Console.WriteLine("Enter maximum distance: ");
                double v1maxDistance = double.Parse(Console.ReadLine()!);
                Console.WriteLine(@"Enter type of distance (0- driving, 1- walking, 2- air distance: ");
                TypeOfDistance v1distanceType = (TypeOfDistance)int.Parse(Console.ReadLine()!);

                Volunteer v1 = new Volunteer
                {
                    Id = id,
                    Name = v1name,
                    Role = v1role,
                    Phone = v1phoneNumber,
                    Email = v1email,
                    Password = v1password,
                    Address = v1address,
                    MaxDistance = v1maxDistance,
                    DistanceType = v1distanceType,
                    IsActive = true
                };
                s_bl.Volunteer.UpdateVolunteerDetails(id, v1);
                Console.WriteLine("Volunteer Updated Successfully");

            }
            else
                Console.WriteLine("Volunteer not found.");
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    private static void DeleteVolunteer()
    {
        Console.Write("Enter volunteer ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int myId))
        {
            s_bl.Volunteer.DeletingVolunteer(myId);
            Console.WriteLine("Volunteer deleted.");
        }
        else
        {
            Console.WriteLine("It's not possible : Invalid ID format.");
        }
    }

    /// <summary>
    /// Allows a volunteer to log into the system.
    /// </summary>
    private static void VolunteerLogin()
    {
        Console.WriteLine("Enter Volunteer name: ");
        string? name = Console.ReadLine();
        Console.WriteLine("Enter your password: ");
        string? myPassword = Console.ReadLine();
        var myRole = s_bl.Volunteer.EnteredSystem(name, myPassword);
        Console.WriteLine(myRole);
    }

    /// <summary>
    /// Displays the Call submenu and handles user choices for call management operations.
    /// </summary>
    private static void CallSubMenu()
    {
        PrintSubMenu("Call");
        int subChoice = int.Parse(Console.ReadLine()!);

        while (subChoice > 0)
        {
            try
            {
                switch (subChoice)
                {
                    case 0:
                        //exit
                        return;
                    case 1:
                        //create
                        AddCall();
                        break;
                    case 2:
                        //read
                        CallDetails();
                        break;
                    case 3:
                        //read all
                        AllCalls();
                        break;
                    case 4:
                        //update
                        UpdateCallSubMenu();
                        break;
                    case 5:
                        //delete
                        DeleteCall();
                        break;
                    case 6:
                        //all closed calls
                        AllClosedCalls();
                        break;
                    case 7:
                        //all open calls
                        AllOpenCalls();
                        break;
                    case 8:
                        //number of calls per status
                        NumberOfCallsByStatus();
                        break;
                    case 9:
                        //assign call to volunteer
                        AssignCall();
                        break;
                }

            }
            catch (Exception ex) { Console.WriteLine($"Error:{ex}"); }
            PrintSubMenu("Call");
        }
    }

    /// <summary>
    /// Adds a new call to the system.
    /// Prompts the user for call details such as ID, type, description, address, location, and time constraints.
    /// </summary>
    private static void AddCall()
    {
        Console.WriteLine("Enter call ID: ");
        int c1id = int.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Enter call type (0- fixing, 1- cooking, 2- babysitting, 3- cleaning, 4- shopping): ");
        CallType c1callType = (CallType)int.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Enter description (if necessary): ");
        string c1description = Console.ReadLine()!;
        Console.WriteLine("Enter call address: ");
        string c1address = Console.ReadLine()!;
        Console.WriteLine("Enter longitude: ");
        double c1latitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter latitude: ");
        double c1longitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine(@"Enter latest time for call to be taken: ");
        DateTime c1maxTime = DateTime.Parse(Console.ReadLine()!);

        Call c1 = new Call
        {
            Id = c1id,
            CallType = c1callType,
            CallDescription = c1description,
            CallAddress = c1address,
            Latitude = c1latitude,
            Longitude = c1longitude,
            MaxEndCallTime = c1maxTime,
            StartCallTime = s_bl.Admin.GetClock()
        };

        s_bl.Call.AddingCall(c1);
        Console.WriteLine("Call Added Successfully");
    }

    /// <summary>
    /// Retrieves and displays details of a specific call by its ID.
    /// </summary>
    private static void CallDetails()
    {
        Console.WriteLine("Enter call ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var myCall = s_bl.Volunteer.GetVolunteerDetails(id);
            Console.WriteLine(myCall != null ? myCall.ToString() : "Call not found.");
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }

    }

    /// <summary>
    /// Displays a list of all calls in the system.
    /// </summary>
    private static void AllCalls()
    {
        if (s_bl!.Call != null)
        {
            foreach (var isCall in s_bl!.Call.GetCallsList())
            {
                Console.WriteLine(isCall);
            }
        }
    }

    /// <summary>
    /// Displays a list of all closed calls in the system.
    /// Iterates through volunteers and their respective closed calls.
    /// </summary>
    private static void AllClosedCalls()
    {
        if (s_bl!.Volunteer != null)
        {
            foreach (var Vol in s_bl!.Volunteer.GetVolunteersList(null, null))
            {
                foreach (var voCall in s_bl.Call.GetClosedCallsByVolunteer(Vol.Id, null, null))
                {
                    Console.WriteLine(voCall);
                }
            }
        }
    }

    /// <summary>
    /// Displays all open calls for every volunteer in the system.
    /// </summary>
    private static void AllOpenCalls()
    {
        if (s_bl!.Volunteer != null)
        {
            foreach (var Vol in s_bl!.Volunteer.GetVolunteersList(null, null))
            {
                foreach (var voCall in s_bl.Call.GetOpenCallsForVolunteer(Vol.Id, null, null))
                {
                    Console.WriteLine(voCall);
                }
            }
        }
    }

    /// <summary>
    /// Updates an existing call's details based on user input.
    /// </summary>
    private static void UpdateCall()
    {
        Console.WriteLine("Enter Call ID: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (s_bl!.Call.GetCallsDetails(id) != null)
            {

                Console.WriteLine(s_bl!.Call.GetCallsDetails(id));

                Console.WriteLine(@"Enter call type (0- fixing, 1- cooking, 2- babysitting, 3- cleaning, 4- shopping): ");
                CallType c1callType = (CallType)int.Parse(Console.ReadLine()!);
                Console.WriteLine(@"Enter description (if necessary): ");
                string c1description = Console.ReadLine()!;
                Console.WriteLine("Enter call address: ");
                string c1address = Console.ReadLine()!;
                Console.WriteLine("Enter longitude: ");
                double c1latitude = double.Parse(Console.ReadLine()!);
                Console.WriteLine("Enter latitude: ");
                double c1longitude = double.Parse(Console.ReadLine()!);
                Console.WriteLine(@"Enter latest time for call to be taken: ");
                DateTime c1maxTime = DateTime.Parse(Console.ReadLine()!);

                Call c1 = new Call
                {
                    Id = id,
                    CallType = c1callType,
                    CallDescription = c1description,
                    CallAddress = c1address,
                    Latitude = c1latitude,
                    Longitude = c1longitude,
                    MaxEndCallTime = c1maxTime,
                    StartCallTime = s_bl.Admin.GetClock()
                };

                s_bl!.Call.UpdateCallDetails(c1);
                Console.WriteLine("Call Updated Successfully");

            }
            else
                Console.WriteLine("Call not found.");
        }
        else
        {
            Console.WriteLine("Invalid ID format.");
        }
    }

    /// <summary>
    /// Marks a call as completed for a specified volunteer and assignment.
    /// </summary>
    private static void UpdateFinishTreatment()
    {
        Console.WriteLine("Enter Volunteer ID: ");
        int.TryParse(Console.ReadLine(), out int id);
        Console.WriteLine("Enter Assignment ID: ");
        int.TryParse(Console.ReadLine(), out int myId);
        s_bl.Call.ReportCallCompletion(id, myId);
    }

    /// <summary>
    /// Cancels an ongoing treatment for a specified volunteer and assignment.
    /// </summary>
    private static void UpdateCancelTreatment()
    {
        Console.WriteLine("Enter Volunteer ID: ");
        int.TryParse(Console.ReadLine(), out int id);
        Console.WriteLine("Enter Assignment ID: ");
        int.TryParse(Console.ReadLine(), out int myId);
        s_bl.Call.CancelCallHandling(id, myId);
    }

    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    private static void DeleteCall()
    {
        Console.Write("Enter call ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int myId))
        {
            s_bl!.Call.DeletingCall(myId);
            Console.WriteLine("Call deleted.");
        }
        else
        {
            Console.WriteLine("It's not possible : Invalid ID format.");
        }
    }

    /// <summary>
    /// Prints the number of calls by their statuses.
    /// </summary>
    private static void NumberOfCallsByStatus()
    {
        var tmp = s_bl.Call.RequestCallsQuantities();
        for (int i = 0; i < tmp.Length; i++)
        {
            Console.WriteLine((CallType)i + ": " + tmp[i]);
        }
    }

    /// <summary>
    /// Assigns a call to a volunteer based on user input.
    /// </summary>
    private static void AssignCall()
    {
        Console.WriteLine("Enter Call ID: ");
        int a1cId = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer ID: ");
        int a1vId = int.Parse(Console.ReadLine()!);
        s_bl!.Call.AssignCallToVolunteer(a1vId, a1cId);
    }

    /// <summary>
    /// Prints the administrator submenu options for call management.
    /// </summary>
    private static void PrintAdminSubMenu()
    {
        Console.Write(@"Call Menu:
1- Advance Clock
2- View Clock
3- set Riks Range
4- View Risk Range
0- exit
Enter your choice: 
");
    }

    /// <summary>
    /// Handles the administrator submenu for call management, allowing selection of different operations.
    /// </summary>
    private static void AdminSubMenu()
    {
        PrintAdminSubMenu();
        int subChoice = int.Parse(Console.ReadLine()!);

        while (subChoice > 0)
        {
            try
            {
                switch (subChoice)
                {
                    case 0:
                        //exit
                        return;
                    case 1:
                        //update clock
                        ForwardClock();
                        break;
                    case 2:
                        //get clock
                        GetClock();
                        break;
                    case 3:
                        //
                        SetRiskRange();
                        break;
                    case 4:
                        //
                        GetRiskRange();
                        break;
                }

            }
            catch (Exception ex) { Console.WriteLine($"Error:{ex}"); }
            PrintAdminSubMenu();
            subChoice = int.Parse(Console.ReadLine()!);
        }

    }

    /// <summary>
    /// Prints the menu for advancing the system clock by specific time intervals.
    /// </summary>
    private static void PrintForwardMenu()
    {
        Console.Write(@" Menu of updating:
0- Advance Seconds
1- Advance Minutes
2- Advance Hours
3- Advance Days
4- Advance Months
5- Advance Years
Enter your choice: 
");
    }

    /// <summary>
    /// Advances the system clock by a specified time unit. Provides options for the user to select which unit of time to advance.
    /// </summary>
    private static void ForwardClock()
    {
        PrintForwardMenu();
        int subChoice = int.Parse(Console.ReadLine()!);
        try
        {
            switch (subChoice)
            {
                case 0:
                    // second
                    s_bl.Admin.AdvanceClock(TimeUnit.SECOND);
                    return;
                case 1:
                    // minute
                    s_bl.Admin.AdvanceClock(TimeUnit.MINUTE);
                    break;
                case 2:
                    //hour
                    s_bl.Admin.AdvanceClock(TimeUnit.HOUR);
                    break;
                case 3:
                    //day
                    s_bl.Admin.AdvanceClock(TimeUnit.DAY);
                    break;
                case 4:
                    //month
                    s_bl.Admin.AdvanceClock(TimeUnit.MONTH);
                    break;
                case 5:
                    //year
                    s_bl.Admin.AdvanceClock(TimeUnit.YEAR);
                    break;
            }
        }
        catch (Exception ex) { Console.WriteLine($"Error:{ex}"); }
    }

    /// <summary>
    /// Initializes the database by calling the initialization method in the business logic layer.
    /// </summary>
    private static void InitializeDB()
    {
        s_bl.Admin.InitializeDatabase();
        Console.WriteLine("DataBase Initialized");
    }

    /// <summary>
    /// Resets the database by calling the reset method in the business logic layer.
    /// </summary>
    private static void ResetDB()
    {
        s_bl.Admin.ResetDatabase();
        Console.WriteLine("DataBase Reset");
    }

    /// <summary>
    /// Retrieves the current risk range from the system by calling the respective method in the business logic layer.
    /// </summary>
    private static void GetRiskRange()
    {

        s_bl.Admin.GetRiskTimeRange();
    }

    /// <summary>
    /// Sets the risk range for the system. Prompts the user for input in the format hh:mm:ss and applies it if valid.
    /// </summary>
    private static void SetRiskRange()
    {
        Console.WriteLine("Enter Risk Range (format hh:mm:ss): ");
        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan riskRange))
        {
            s_bl.Admin.SetRiskTimeRange(riskRange);
            Console.WriteLine($"Risk range set to: {riskRange}");
        }
        else
        {
            Console.WriteLine("Invalid time format. Please use hh:mm:ss.");
        }
    }

    /// <summary>
    /// Displays the current system clock time by calling the respective method in the business logic layer.
    /// </summary>
    private static void GetClock()
    {
        Console.WriteLine("Clock: " + s_bl.Admin.GetClock());
    }

}

