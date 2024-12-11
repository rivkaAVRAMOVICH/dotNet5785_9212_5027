using Dal;
using DalApi;
using DO;
using System;
using System.Data;
using System.Xml.Linq;

namespace DalTest
{
    internal class Program
    {
       
        static readonly IDal s_dal = new DalList();
        enum MainMenuOptions
        {
            Exit,
            VolunteerSubMenu,
            CallSubMenu,
            AssignmentSubMenu,
            InitializeData,
            DisplayAllData,
            ConfigSubMenu,
            ResetDatabaseAndConfig
        }

        enum SubMenuOptions
        {
            Exit,
            Create,
            Read,
            ReadAll,
            Update,
            Delete,
            DeleteAll
        }

        enum ConfigMenuOptions
        {
            Exit,
            AdvanceClockMinute,
            AdvanceClockHour,
            DisplayClock,
            SetNewConfigValue,
            DisplayConfigValue,
            ResetConfig
        }
        //used chatGPT
        static void Main(string[] args)
        {
            bool exit = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Main Menu:");
                foreach (MainMenuOptions option in Enum.GetValues(typeof(MainMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Select an option: ");
                if (Enum.TryParse(Console.ReadLine(), out MainMenuOptions choice))
                {
                    switch (choice)
                    {
                        case MainMenuOptions.Exit: exit = true; break;
                        case MainMenuOptions.VolunteerSubMenu: DisplayVolunteerSubMenu(); break;
                        case MainMenuOptions.CallSubMenu: DisplayCallSubMenu(); break;
                        case MainMenuOptions.AssignmentSubMenu: DisplayAssignmentSubMenu(); break;
                        case MainMenuOptions.InitializeData: InitializeData(); break;
                        case MainMenuOptions.DisplayAllData: DisplayAllData(); break;
                        case MainMenuOptions.ConfigSubMenu: DisplayConfigSubMenu(); break;
                        case MainMenuOptions.ResetDatabaseAndConfig: ResetDatabaseAndConfig(); break;
                        default:
                            Console.WriteLine("Invalid choice.");
                            Pause();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    Pause();
                }
            } while (!exit);
        }
        //used chatGPT
        private static void DisplayAssignmentSubMenu()
        {
            bool exit = false;
            do
            {
                Console.Clear();
                Console.WriteLine("Sub-Menu for Assignment:");
                foreach (SubMenuOptions option in Enum.GetValues(typeof(SubMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Select an option: ");
                if (Enum.TryParse(Console.ReadLine(), out SubMenuOptions choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case SubMenuOptions.Exit:
                                exit = true;
                                break;
                            case SubMenuOptions.Create:
                                CreateAssignment();
                                break;
                            case SubMenuOptions.Read:
                                ReadAssignment();
                                break;
                            case SubMenuOptions.ReadAll:
                                ReadAllAssignments();
                                break;
                            case SubMenuOptions.Update:
                                UpdateAssignment();
                                break;
                            case SubMenuOptions.Delete:
                                DeleteAssignment();
                                break;
                            case SubMenuOptions.DeleteAll:
                                DeleteAllAssignments();
                                break;
                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Pause();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    Pause();
                }
            } while (!exit);
        }
     
        private static void DisplayVolunteerSubMenu()
        {
            bool exit = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Sub-Menu for Volunteer:");
                foreach (SubMenuOptions option in Enum.GetValues(typeof(SubMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Select an option: ");
                if (Enum.TryParse(Console.ReadLine(), out SubMenuOptions choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case SubMenuOptions.Exit:
                                exit = true;
                                break;

                            case SubMenuOptions.Create:
                                CreateVolunteer();
                                break;

                            case SubMenuOptions.Read:
                                ReadVolunteer();
                                break;

                            case SubMenuOptions.ReadAll:
                                ReadAllVolunteers();
                                break;

                            case SubMenuOptions.Update:
                                UpdateVolunteer();
                                break;

                            case SubMenuOptions.Delete:
                                DeleteVolunteer();
                                break;

                            case SubMenuOptions.DeleteAll:
                                DeleteAllVolunteers();
                                break;

                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Pause();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    Pause();
                }
            } while (!exit);
        }
        //used chatGPT
        private static void DisplayCallSubMenu()
        {
            bool exit = false;

            do
            {
                Console.Clear();
                Console.WriteLine("Sub-Menu for Call:");
                foreach (SubMenuOptions option in Enum.GetValues(typeof(SubMenuOptions)))
                {
                    Console.WriteLine($"{(int)option}. {option}");
                }

                Console.Write("Select an option: ");
                if (Enum.TryParse(Console.ReadLine(), out SubMenuOptions choice))
                {
                    try
                    {
                        switch (choice)
                        {
                            case SubMenuOptions.Exit:
                                exit = true;
                                break;

                            case SubMenuOptions.Create:
                                CreateCall();
                                break;

                            case SubMenuOptions.Read:
                                ReadCall();
                                break;

                            case SubMenuOptions.ReadAll:
                                ReadAllCalls();
                                break;

                            case SubMenuOptions.Update:
                                UpdateCall();
                                break;

                            case SubMenuOptions.Delete:
                                DeleteCall();
                                break;

                            case SubMenuOptions.DeleteAll:
                                DeleteAllCalls();
                                break;

                            default:
                                Console.WriteLine("Invalid choice.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Pause();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                    Pause();
                }
            } while (!exit);
        }

        private static void Pause()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }


        private static void CreateCall()
        {
            Console.WriteLine("Creating a new Call...");

            try
            {
                Console.Write("Enter Call ID: ");
                int id = int.Parse(Console.ReadLine());


                Console.WriteLine("Enter Kind of Call (0- Emergency, 1- Assistance, 2- Inquiry, 3- Other): ");
                Enums.CallTypeEnum kindOfCall = (Enums.CallTypeEnum)int.Parse(Console.ReadLine());

                Console.Write("Enter Full Address: ");
                string fullAddress = Console.ReadLine();

                Console.Write("Enter Longitude: ");
                double longitude = double.Parse(Console.ReadLine());

                Console.Write("Enter Latitude: ");
                double latitude = double.Parse(Console.ReadLine());

                Console.Write("Enter Opening Hour (yyyy-MM-dd HH:mm:ss): ");
                DateTime openHour = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Closing Hour (yyyy-MM-dd HH:mm:ss): ");
                DateTime closeHour = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Description (optional): ");
                string? description = Console.ReadLine();

                // Create the Call object
                Call call = new Call
                {
                    Id = id,
                    FullAddressCall = fullAddress,
                    Longitude = longitude,
                    Latitude = latitude,
                    openTime = openHour,
                    CallType = kindOfCall,
                    MaxTimeFinish = closeHour
                };

                // Save the Call using DAL
                s_dal.Call?.Create(call);
                Console.WriteLine("Call created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating call: {ex.Message}");
            }

            Pause();
        }


        private static void ReadCall()
        {
            Console.Write("Enter Call ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var call = s_dal.Call?.Read(id);
                if (call != null)
                {
                    Console.WriteLine(call);
                }
                else
                {
                    Console.WriteLine("Call not found.");
                }
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void ReadAllCalls()
        {
            Console.WriteLine("Displaying all Calls:");
            var calls = s_dal.Call?.ReadAll();
            if (calls != null)
            {
                foreach (var call in calls)
                {
                    Console.WriteLine(call);
                }
            }
            else
            {
                Console.WriteLine("No Calls found.");
            }
            Pause();
        }

        private static void UpdateCall()
        {
            Console.WriteLine("Updating a Call...");

            Console.Write("Enter Call ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("Enter Kind of Call (0- Emergency, 1- Assistance, 2- Inquiry, 3- Other): ");
            Enums.CallTypeEnum kindOfCall = (Enums.CallTypeEnum)int.Parse(Console.ReadLine());

            Console.Write("Enter Full Address: ");
            string fullAddress = Console.ReadLine();

            Console.Write("Enter Longitude: ");
            double longitude = double.Parse(Console.ReadLine());

            Console.Write("Enter Latitude: ");
            double latitude = double.Parse(Console.ReadLine());

            Console.Write("Enter Opening Hour (yyyy-MM-dd HH:mm:ss): ");
            DateTime openHour = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Closing Hour (yyyy-MM-dd HH:mm:ss): ");
            DateTime closeHour = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Description (optional): ");
            string? description = Console.ReadLine();

            // Update the Call object
            Call call = new Call
            {
                Id = id,
                FullAddressCall = fullAddress,
                Longitude = longitude,
                Latitude = latitude,
                openTime = openHour,
                CallType = kindOfCall,
                MaxTimeFinish = closeHour
            };

            s_dal.Call?.Update(call);
            Console.WriteLine("Call updated.");
            Pause();
        }


        private static void DeleteCall()
        {
            Console.Write("Enter Call ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dal.Call?.Delete(id);
                Console.WriteLine("Call deleted.");
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void DeleteAllCalls()
        {
            Console.WriteLine("Deleting all Calls...");
            s_dal.Call?.DeleteAll();
            Console.WriteLine("All Calls deleted.");
            Pause();
        }
        private static void CreateVolunteer()
        {
            Console.WriteLine("Creating a new Volunteer...");

            try
            {
                Console.Write("Enter Volunteer ID: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Enter Volunteer Name: ");
                string name = Console.ReadLine();

                Console.Write("Enter Phone Number: ");
                string phoneNo = Console.ReadLine();

                Console.Write("Enter Email: ");
                string email = Console.ReadLine();

                Console.Write("Enter Role (1- Driver, 2- Organizer, 3- Other): ");
                Enums.RoleEnum role = (Enums.RoleEnum)Enum.Parse(typeof(Enums.RoleEnum), Console.ReadLine(), true);

                Console.Write("Is the Volunteer Active? (true/false): ");
                bool active = bool.Parse(Console.ReadLine());

                Console.Write("Enter Distance (in km, or press 0): ");
                string distanceInput = Console.ReadLine();
                Enums.DistanceTypeEnum distance = (Enums.DistanceTypeEnum)int.Parse(distanceInput);

                Console.Write("Enter Password (optional): ");
                string? password = Console.ReadLine();

                Console.Write("Enter Current Address (optional): ");
                string? currentAddress = Console.ReadLine();

                Console.Write("Enter Latitude (optional, or press Enter to skip): ");
                string latitudeInput = Console.ReadLine();
                double? latitude = string.IsNullOrEmpty(latitudeInput) ? null : double.Parse(latitudeInput);

                Console.Write("Enter Longitude (optional, or press Enter to skip): ");
                string longitudeInput = Console.ReadLine();
                double? longitude = string.IsNullOrEmpty(longitudeInput) ? null : double.Parse(longitudeInput);

                Console.Write("Enter Farthest Distance (optional, or press Enter to skip): ");
                string farthestDistanceInput = Console.ReadLine();
                double? farthestDistance = string.IsNullOrEmpty(farthestDistanceInput) ? null : double.Parse(farthestDistanceInput);

                // Create the volunteer object
                Volunteer volunteer = new Volunteer
                {
                    Id = id,
                    FullName = name,
                    PhoneNumber = phoneNo,
                    Email = email,
                    Role = role,
                    Active = active,
                    DistanceType = distance,
                    Password = password,
                    FullAddress = currentAddress,
                    Latitude = latitude,
                    Longitude = longitude,
                    MaxDistance = farthestDistance
                };

                // Save the volunteer using DAL
                s_dal.Volunteer?.Create(volunteer);
                Console.WriteLine("Volunteer created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating volunteer: {ex.Message}");
            }

            Pause();
        }


        private static void ReadVolunteer()
        {
            Console.Write("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var volunteer = s_dal.Volunteer?.Read(id);
                if (volunteer != null)
                {
                    Console.WriteLine(volunteer);
                }
                else
                {
                    Console.WriteLine("Volunteer not found.");
                }
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void ReadAllVolunteers()
        {
            Console.WriteLine("Displaying all Volunteers:");
            var volunteers = s_dal.Volunteer?.ReadAll();
            if (volunteers != null)
            {
                foreach (var volunteer in volunteers)
                {
                    Console.WriteLine(volunteer);
                }
            }
            else
            {
                Console.WriteLine("No Volunteers found.");
            }
            Pause();
        }

        private static void UpdateVolunteer()
        {

            Console.Write("Enter Volunteer ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Volunteer Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter Phone Number: ");
            string phoneNo = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Role (1- Driver, 2- Organizer, 3- Other): ");
            Enums.RoleEnum role = (Enums.RoleEnum)Enum.Parse(typeof(Enums.RoleEnum), Console.ReadLine(), true);

            Console.Write("Is the Volunteer Active? (true/false): ");
            bool active = bool.Parse(Console.ReadLine());

            Console.Write("Enter Distance (in km, or press 0): ");
            string distanceInput = Console.ReadLine();
            Enums.DistanceTypeEnum distance = (Enums.DistanceTypeEnum)int.Parse(distanceInput);

            Console.Write("Enter Password (optional): ");
            string? password = Console.ReadLine();

            Console.Write("Enter Current Address (optional): ");
            string? currentAddress = Console.ReadLine();

            Console.Write("Enter Latitude (optional, or press Enter to skip): ");
            string latitudeInput = Console.ReadLine();
            double? latitude = string.IsNullOrEmpty(latitudeInput) ? null : double.Parse(latitudeInput);

            Console.Write("Enter Longitude (optional, or press Enter to skip): ");
            string longitudeInput = Console.ReadLine();
            double? longitude = string.IsNullOrEmpty(longitudeInput) ? null : double.Parse(longitudeInput);

            Console.Write("Enter Farthest Distance (optional, or press Enter to skip): ");
            string farthestDistanceInput = Console.ReadLine();
            double? farthestDistance = string.IsNullOrEmpty(farthestDistanceInput) ? null : double.Parse(farthestDistanceInput);

            // Create the volunteer object
            Volunteer item = new Volunteer
            {
                Id = id,
                FullName = name,
                PhoneNumber = phoneNo,
                Email = email,
                Role = role,
                Active = active,
                DistanceType = distance,
                Password = password,
                FullAddress = currentAddress,
                Latitude = latitude,
                Longitude = longitude,
                MaxDistance = farthestDistance
            };
            // Update fields of the volunteer based on user input
            s_dal.Volunteer?.Update(item);
            Console.WriteLine("Volunteer updated.");


        }

        private static void DeleteVolunteer()
        {
            Console.Write("Enter Volunteer ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dal.Volunteer?.Delete(id);
                Console.WriteLine("Volunteer deleted.");
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void DeleteAllVolunteers()
        {
            Console.WriteLine("Deleting all Volunteers...");
            s_dal.Volunteer?.DeleteAll();
            Console.WriteLine("All Volunteers deleted.");
            Pause();
        }
        private static void CreateAssignment()
        {
            Console.WriteLine("Creating a new Assignment...");

            try
            {
                Console.Write("Enter Assignment ID: ");
                int id = int.Parse(Console.ReadLine());

                Console.Write("Enter Call ID: ");
                int callId = int.Parse(Console.ReadLine());

                Console.Write("Enter Volunteer ID: ");
                int volunteerId = int.Parse(Console.ReadLine());

                Console.Write("Enter Assignment Start Date and Time (yyyy-MM-dd HH:mm:ss): ");
                DateTime assignmentStart = DateTime.Parse(Console.ReadLine());

                Console.Write("Enter Assignment End Date and Time (optional, yyyy-MM-dd HH:mm:ss, or press Enter to skip): ");
                string assignmentEndInput = Console.ReadLine();
                DateTime? assignmentEnd = string.IsNullOrEmpty(assignmentEndInput)
                    ? null
                    : DateTime.Parse(assignmentEndInput);

                Console.Write("Enter Kind of End Assignment (0- Completed, 1- Cancelled, 2- Expired, or press Enter to skip): ");
                string kindOfEndInput = Console.ReadLine();
                Enums.finishTreatmentTypeEnum? kindOfEndAssignment = string.IsNullOrEmpty(kindOfEndInput)
                    ? null
                    : (Enums.finishTreatmentTypeEnum)int.Parse(kindOfEndInput);

                // Create the Assignment object
                Assignment assignment = new Assignment
                {
                    Id = id,
                    CallId = callId,
                    VolunteerId = volunteerId,
                    EnteryTimeTreatment = assignmentStart,
                    FinushTimeTreatment = assignmentEnd,
                    FinishTreatmentType = kindOfEndAssignment
                };

                // Save the Assignment using DAL
                s_dal.Assignment?.Create(assignment);
                Console.WriteLine("Assignment created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating assignment: {ex.Message}");
            }

            Pause();
        }

        private static void ReadAssignment()
        {
            Console.Write("Enter Assignment ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var assignment = s_dal.Assignment?.Read(id);
                if (assignment != null)
                {
                    Console.WriteLine(assignment);
                }
                else
                {
                    Console.WriteLine("Assignment not found.");
                }
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void ReadAllAssignments()
        {
            Console.WriteLine("Displaying all Assignments:");
            var assignments = s_dal.Assignment?.ReadAll();
            if (assignments != null)
            {
                foreach (var assignment in assignments)
                {
                    Console.WriteLine(assignment);
                }
            }
            else
            {
                Console.WriteLine("No Assignments found.");
            }
            Pause();
        }

        private static void UpdateAssignment()
        {
            Console.WriteLine("Updating an Assignment...");

            Console.Write("Enter Assignment ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Call ID: ");
            int callId = int.Parse(Console.ReadLine());

            Console.Write("Enter Volunteer ID: ");
            int volunteerId = int.Parse(Console.ReadLine());

            Console.Write("Enter Assignment Start Date and Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime assignmentStart = DateTime.Parse(Console.ReadLine());

            Console.Write("Enter Assignment End Date and Time (optional, yyyy-MM-dd HH:mm:ss, or press Enter to skip): ");
            string assignmentEndInput = Console.ReadLine();
            DateTime? assignmentEnd = string.IsNullOrEmpty(assignmentEndInput)
                ? null
                : DateTime.Parse(assignmentEndInput);

            Console.Write("Enter Kind of End Assignment (0- Completed, 1- Cancelled, 2- Expired, or press Enter to skip): ");
            string kindOfEndInput = Console.ReadLine();
            Enums.finishTreatmentTypeEnum? kindOfEndAssignment = string.IsNullOrEmpty(kindOfEndInput)
                ? null
                : (Enums.finishTreatmentTypeEnum)int.Parse(kindOfEndInput);

            // Update the Assignment object
            Assignment assignment = new Assignment
            {
                Id = id,
                CallId = callId,
                VolunteerId = volunteerId,
                EnteryTimeTreatment = assignmentStart,
                FinushTimeTreatment = assignmentEnd,
                FinishTreatmentType = kindOfEndAssignment
            };

            s_dal.Assignment?.Update(assignment);
            Console.WriteLine("Assignment updated.");
            Pause();
        }


        private static void DeleteAssignment()
        {
            Console.Write("Enter Assignment ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                s_dal.Assignment?.Delete(id);
                Console.WriteLine("Assignment deleted.");
                Pause();
            }
            else
            {
                Console.WriteLine("Invalid ID.");
                Pause();
            }
        }

        private static void DeleteAllAssignments()
        {
            Console.WriteLine("Deleting all Assignments...");
            s_dal.Assignment?.DeleteAll();
            Console.WriteLine("All Assignments deleted.");
            Pause();
        }
        //used chatGPT
        private static void DisplayConfigSubMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("===== Configuration Menu =====");
                Console.WriteLine("1. Advance System Clock by One Minute");
                Console.WriteLine("2. Advance System Clock by One Hour");
                Console.WriteLine("3. Display Current System Clock");
                Console.WriteLine("4. Set a New Clock Value");
                Console.WriteLine("5. Reset All Configuration Variables");
                Console.WriteLine("0. Exit Configuration Menu");
                Console.Write("Select an option: ");

                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            AdvanceClockBy(TimeSpan.FromMinutes(1));
                            break;
                        case 2:
                            AdvanceClockBy(TimeSpan.FromHours(1));
                            break;
                        case 3:
                            DisplaySystemClock();
                            break;
                        case 4:
                            SetClockValue();
                            break;
                        case 5:
                            ResetAllConfigVariables();
                            break;
                        case 0:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            Pause();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    Pause();
                }
            }
        }
        private static void ResetAllConfigVariables()
        {
            var config = new ConfigImplementation();
            config.Reset();
            Console.WriteLine("All configuration variables have been reset.");
            Pause();
        }
        private static void DisplaySystemClock()
        {
            Console.WriteLine($"Current System Clock: {s_dal.Config.Clock}");
            Pause();
        }
        private static void AdvanceClockBy(TimeSpan timeSpan)
        {
            s_dal.Config.Clock += timeSpan;
            Console.WriteLine($"System clock advanced by {timeSpan.TotalMinutes} minutes.");
            Pause();
        }

        private static void SetClockValue()
        {
            Console.Write("Enter the new clock value (format: yyyy-MM-dd HH:mm:ss): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime newClockValue))
            {
                s_dal.Config.Clock = newClockValue;
                Console.WriteLine($"System clock updated to: {s_dal.Config.Clock}");
            }
            else
            {
                Console.WriteLine("Invalid date format. Please try again.");
            }
            Pause();
        }

        private static void InitializeData()
        {
            Console.WriteLine("Initializing data...");
            Initialization.Do(s_dal);
            Pause();
        }

        private static void DisplayAllData()
        {
            Console.WriteLine("Displaying all data...");
            ReadAllVolunteers();
            ReadAllCalls();
            ReadAllAssignments();
            Pause();
        }

        // Implementation for resetting database and config
        private static void ResetDatabaseAndConfig()
        {
            Console.WriteLine("Resetting database and configuration...");
            DeleteAllAssignments();
            DeleteAllCalls();
            DeleteAllVolunteers();
            s_dal.Config.Reset();
            Pause();
        }
    }
}
