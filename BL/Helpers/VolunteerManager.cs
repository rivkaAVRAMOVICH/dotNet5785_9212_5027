﻿//using BO;
using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Net;


namespace Helpers
{
    internal static class VolunteerManager
    {
        private static IDal s_dal = DalApi.Factory.Get; // Access to the DAL layer
        internal static ObserverManager Observers = new(); //stage 5 
        private static readonly Random s_rand = new();
        /// <summary>
        /// Validates an Israeli ID using a checksum algorithm.
        /// </summary>
        internal static bool isId(int id)
        {
            int sum = 0;

            if (id > 999999999 || id < 100000000)
                return false;

            for (int i = 0; i < 9; i++)
            {
                int digit = id % 10;
                id /= 10;
                int multiplied = digit * ((i % 2) + 1);
                sum += (multiplied > 9) ? multiplied - 9 : multiplied;
            }

            return sum % 10 == 0;
        }

        /// <summary>
        /// Validates if the given string is a valid email address format.
        /// </summary>
        internal static bool isEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, emailPattern);
        }

        /// <summary>
        /// Validates the given phone number format.
        /// </summary>
        internal static bool isPhone(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return false;

            string phonePattern = @"^\+?\d{1,4}?[-.\s]?(\(?\d{1,3}?\)?[-.\s]?)?\d{3,4}[-.\s]?\d{4}$";

            return Regex.IsMatch(number, phonePattern);
        }

        /// <summary>
        /// Checks if the password meets strength requirements.
        /// </summary>
        internal static bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"[A-Z]") &&
                   Regex.IsMatch(password, @"[a-z]") &&
                   Regex.IsMatch(password, @"[0-9]") &&
                   Regex.IsMatch(password, @"[\W_]");
        }

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Verifies that a plain password matches the hashed version.
        /// </summary>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        /// <summary>
        /// Converts a list of DO.Volunteer objects to BO.VolunteerInList with calculated properties.
        /// </summary>
        public static IEnumerable<BO.VolunteerInList> ConvertVolunteersToBO(IEnumerable<DO.Volunteer> volunteers)
        {
            return volunteers.Select(volunteer => new BO.VolunteerInList
            {
                Id = volunteer.Id,
                Name = volunteer.FullName,
                IsActive = volunteer.Active,
                SumOfHandleCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == volunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.Treated),
                SumCanceledCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == volunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.SelfCancellation),
                SumOfExpiredCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == volunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.ExpiredCancellation),
                IdCallInProgress = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.FinishTimeTreatment == null)?.CallId,
                TypeCallInProgress = s_dal.Assignment.ReadAll()
                    .FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.FinishTimeTreatment == null) is DO.Assignment aInProgress &&
                    s_dal.Call.Read(aInProgress.CallId) is DO.Call c
                    ? (BO.CallType)c.CallType
                    : BO.CallType.none
            });
        }

        /// <summary>
        /// Converts a BO.Volunteer object to a DO.Volunteer object including geolocation.
        /// </summary>
        public static DO.Volunteer ConvertVolunteersToDo(BO.Volunteer volunteer)
        {
            var finalVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                Password = HashPassword(volunteer.Password),
                FullName = volunteer.Name,
                Email = volunteer.Email,
                PhoneNumber = volunteer.Phone,
                FullAddress = volunteer.Address,
                Latitude = Tools.GetLatitudLongitutes(volunteer.Address).Latitude,
                Longitude = Tools.GetLatitudLongitutes(volunteer.Address).Longitude,
                Role = (DO.RoleEnum)volunteer.Role
            };
            return finalVolunteer;
        }

        /// <summary>
        /// Creates a BO.Volunteer object with call details and statistics from a DO.Volunteer.
        /// </summary>
        public static BO.Volunteer addNewVolunteerWithCall(DO.Volunteer tmpVolunteer)
        {
            BO.Volunteer final = new BO.Volunteer()
            {
                Id = tmpVolunteer.Id,
                Name = tmpVolunteer.FullName,
                Phone = tmpVolunteer.PhoneNumber,
                Email = tmpVolunteer.Email,
                Latitude = tmpVolunteer.Latitude,
                Longitude = tmpVolunteer.Longitude,
                Password = tmpVolunteer.Password,
                Address = tmpVolunteer.FullAddress,
                Role = (BO.Role)tmpVolunteer.Role,
                IsActive = tmpVolunteer.Active,
                MaxDistance = tmpVolunteer.MaxDistance,
                TypeOfDistance = (BO.TypeOfDistance)tmpVolunteer.DistanceType,
                SumHandledCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.Treated),
                SumCanceledCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.SelfCancellation),
                SumChosenExpiredCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.ExpiredCancellation),
                CallInVolunteerHandle = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == tmpVolunteer.Id && a.FinishTimeTreatment == null) is DO.Assignment aInProgress ? addCall(aInProgress, tmpVolunteer) : null
            };
            return final;
        }

        /// <summary>
        /// Builds a BO.CallInProgress object with detailed status based on timing.
        /// </summary>
        public static BO.CallInProgress addCall(DO.Assignment aInProgress, DO.Volunteer volunteer)
        {
            DO.Call? call;
            lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(aInProgress.CallId);
            BlApi.IAdmin admin = new BlImplementation.AdminImplementation();
            return new BO.CallInProgress
            {
                Id = aInProgress.Id,
                CallId = call.Id,
                CallType = (BO.CallType)call.CallType,
                CallAddress = call.CallAddress,
                CallDescription = call.CallDescription,
                MaxEndTime = call.MaxEndCallTime,
                TimeCallMade = call.StartCallTime,
                EntryTimeTreatment = aInProgress.EntryTimeTreatment,
                DistanceCallFromVolunteer = CalculateDistance(call.Latitude, call.Longitude, (double)volunteer.Latitude, (double)volunteer.Longitude),
                Status = (call.MaxEndCallTime.Value - DateTime.Now) <= admin.GetRiskTimeRange()
                    ? BO.Status.inProgressAtRisk
                    : BO.Status.inProgress
            };
        }

        /// <summary>
        /// Calculates the distance in kilometers between two geo-coordinates using the Haversine formula.
        /// </summary>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371;
            double dLat = Math.PI / 180 * (lat2 - lat1);
            double dLon = Math.PI / 180 * (lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Math.PI / 180 * lat1) * Math.Cos(Math.PI / 180 * lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Validates the given BO.Volunteer object, including ID, email, phone and address.
        /// Throws exceptions if invalid.
        /// </summary>
        public static bool IntegrityChecker(BO.Volunteer v1)
        {
            if (v1 != null)
            {
                if (!isEmail(v1.Email))
                    throw new InvalidException("Invalid email");

                if (!isPhone(v1.Phone))
                    throw new InvalidException("Invalid phone");

                if (!isId(v1.Id))
                    throw new InvalidException("Invalid id");

                return true;
            }
            return false;
        }

        internal static void PeriodicVolunteerUpdates(DateTime oldClock, DateTime newClock)
        {
            bool volunteersUpdated = false;
            List<int> updatedVolunteerIds = new(); // רשימת מתנדבים שנשלחו למשקיפים

            lock (AdminManager.BlMutex)
            {
                var volunteers = s_dal.Volunteer.ReadAll().ToList();

                foreach (var doVolunteer in volunteers)
                {
                    if ((string.IsNullOrWhiteSpace(doVolunteer.PhoneNumber) ||
                         doVolunteer.Latitude == null ||
                         doVolunteer.Longitude == null) &&
                         doVolunteer.Active)
                    {
                        var updatedVolunteer = doVolunteer with { Active = false };
                        s_dal.Volunteer.Update(updatedVolunteer);
                        updatedVolunteerIds.Add(doVolunteer.Id); // נרשום שנעדכן עליו את המשקיף
                        volunteersUpdated = true;
                    }
                }
            }

            // נעדכן את המשקיפים רק אחרי שהנעילה שוחררה
            foreach (int id in updatedVolunteerIds)
            {
                Observers.NotifyItemUpdated(id);
            }

            if (volunteersUpdated)
            
                Observers.NotifyListUpdated();
        }
        internal static async Task UpdateCoordinatesForVolunteerAsync(DO.Volunteer doVolunteer)
        {
            if (string.IsNullOrWhiteSpace(doVolunteer.FullAddress))
                return;

            var location = await Tools.GetCoordinatesFromAddressAsync(doVolunteer.FullAddress);

            DO.Volunteer updatedVolunteer;

                var (lat, lon) = location.Value;
                updatedVolunteer = doVolunteer with
                {
                    Latitude = lat,
                    Longitude = lon,
                };
            
           

            lock (AdminManager.BlMutex)
            {
                s_dal.Volunteer.Update(updatedVolunteer);
            }

            Observers.NotifyItemUpdated(doVolunteer.Id);
            Observers.NotifyListUpdated();
        }

        internal static async Task SimulateVolunteerActivity()
        {
            Thread.CurrentThread.Name = $"VolunteerSimulatorThread";
            var callManager = new CallImplementation();
            List<DO.Volunteer> volunteers;
            lock (AdminManager.BlMutex)
            {
                volunteers = s_dal.Volunteer.ReadAll(v => v.Active == true).ToList(); // קריאה לרשימה סופית
            }

            List<int> updatedVolunteerIds = new();

            foreach (var doVolunteer in volunteers)
            {
                int volunteerId = doVolunteer.Id;

                // בדיקת האם למתנדב יש כרגע קריאה בטיפול
                DO.Assignment? currentAssignment;
                lock (AdminManager.BlMutex)
                {
                    currentAssignment = s_dal.Assignment.ReadAll()
                        .FirstOrDefault(a => a.VolunteerId == volunteerId && a.FinishTimeTreatment == null);
                }

                if (currentAssignment == null)
                {
                    // אין טיפול כרגע – ננסה להצטרף לטיפול חדש
                    if (s_rand.NextDouble() < 0.2) // 20% הסתברות שיצטרף לטיפול
                    {
                        List<BO.OpenCallInList> possibleCalls;
                        lock (AdminManager.BlMutex)
                        {
                            possibleCalls = callManager.GetOpenCallsForVolunteer(volunteerId,null,null).ToList();
                        }

                        if (possibleCalls.Any())
                        {
                            var selectedCall = possibleCalls[s_rand.Next(possibleCalls.Count)];

                            lock (AdminManager.BlMutex)
                            {
                                callManager.AssignCallToVolunteer(selectedCall.Id, volunteerId);
                            }

                            updatedVolunteerIds.Add(volunteerId);
                        }
                    }
                }
                else
                {
                    // יש קריאה בטיפול – נבחן האם הגיע הזמן לסיים או לבטל

                    DateTime entryTime = currentAssignment.EntryTimeTreatment;
                    double distance = 0;
                   
                        var volunteerCoords = await Tools.GetCoordinatesFromAddressAsync(doVolunteer.FullAddress); // מחזיר (lat, lon)

                    // נניח שיש פונקציה שמחזירה את הכתובת של הקריאה לפי ה-ID שלה
                    lock (AdminManager.BlMutex)
                    {
                        var call = s_dal.Call.Read(currentAssignment.CallId);

                        if (volunteerCoords is null)
                        {
                            return; 
                        }

                        distance = CallManager.GetDistance(
                            volunteerCoords.Value.Latitude,
                            volunteerCoords.Value.Longitude,
                            call.Latitude,
                            call.Longitude
                        );
                    }

                    // סימולציה: מספיק זמן = מרחק בדקות + עד 2 דקות רנדומלי
                    TimeSpan requiredDuration = TimeSpan.FromMinutes(distance + s_rand.Next(1, 3));
                    TimeSpan actualDuration = DateTime.Now - entryTime;

                    if (actualDuration >= requiredDuration)
                    {
                        lock (AdminManager.BlMutex)
                        {
                            callManager.ReportCallCompletion(volunteerId,currentAssignment.CallId);
                        }

                        updatedVolunteerIds.Add(volunteerId);
                    }
                    else
                    {
                        if (s_rand.NextDouble() < 0.1) // 10% הסתברות לביטול
                        {
                            lock (AdminManager.BlMutex)
                            {
                                callManager.CancelCallHandling(currentAssignment.CallId, volunteerId);
                            }

                            updatedVolunteerIds.Add(volunteerId);
                        }
                    }
                }
            }

            // שליחת התראות למשקיפים
            foreach (int id in updatedVolunteerIds)
            {
                Observers.NotifyItemUpdated(id);
            }
        }

    }

}

