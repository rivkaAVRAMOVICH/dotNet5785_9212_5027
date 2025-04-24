//using BO;
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

namespace Helpers
{
    internal static class VolunteerManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        internal static bool isId(int id)
        {
            int sum = 0;

            if (id > 999999999 || id < 100000000)
                return false;

            for (int i = 0; i < 9; i++)
            {
                int digit = id % 10;
                id /= 10; // Removing the right digit
                int multiplied = digit * ((i % 2) + 1); // Alternating multiplication by 1 and 2
                sum += (multiplied > 9) ? multiplied - 9 : multiplied; // Adding the result digits
            }

            return sum % 10 == 0;
        }

        internal static bool isEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, emailPattern);

        }

        internal static bool isPhone(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return false;

            // Regular expression to check phone numbers
            string phonePattern = @"^\+?\d{1,4}?[-.\s]?(\(?\d{1,3}?\)?[-.\s]?)?\d{3,4}[-.\s]?\d{4}$";

            return Regex.IsMatch(number, phonePattern);
        }

        internal static bool IsPasswordStrong(string password)
        {
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"[A-Z]") && // At least one capital letter
                   Regex.IsMatch(password, @"[a-z]") && // At least a tiny one
                   Regex.IsMatch(password, @"[0-9]") && // At least one digit
                   Regex.IsMatch(password, @"[\W_]");  // At least one special character
        }
      
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        
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
                IdCallInProgress = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.FinushTimeTreatment == null)?.CallId,
                TypeCallInProgress = s_dal.Assignment.ReadAll()
    .FirstOrDefault(a => a.VolunteerId == volunteer.Id && a.FinushTimeTreatment == null) is DO.Assignment aInProgress &&
    s_dal.Call.Read(aInProgress.CallId) is DO.Call c
    ? (BO.CallType)c.CallType
    : BO.CallType.None
            });
        }
        public static DO.Volunteer ConvertVolunteersToDo(BO.Volunteer volunteer)
        {
            var finalVolunteer = new DO.Volunteer
            {
                Id = volunteer.Id,
                FullName = volunteer.Name,
                Email = volunteer.Email,
                PhoneNumber = volunteer.Phone,
                FullAddress = volunteer.Address,
                Latitude = CallManager.GetLatitudLongitute(volunteer.Address).Latitude,
                Longitude = CallManager.GetLatitudLongitute(volunteer.Address).Longitude,
                Role =(DO.RoleEnum)volunteer.Role
            };
            return finalVolunteer;
        }


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
                Password = tmpVolunteer.Password, // Initialize password
                Address = tmpVolunteer.FullAddress,
                Role = (BO.Role)tmpVolunteer.Role,
                IsActive = tmpVolunteer.Active,
                MaxDistance = tmpVolunteer.MaxDistance,
                DistanceType = (BO.DistanceType)tmpVolunteer.DistanceType,
                SumHandledCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.Treated),
                SumCanceledCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.SelfCancellation),
                SumChosenExpiredCalls = s_dal.Assignment.ReadAll().Count(a => a.VolunteerId == tmpVolunteer.Id && a.EndTypeAssignment == DO.EndTypeAssignment.ExpiredCancellation),
                CallInVolunteerHandle = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.VolunteerId == tmpVolunteer.Id && a.FinushTimeTreatment == null) is DO.Assignment aInProgress? addCall(aInProgress, tmpVolunteer) : null
            };
            return final;
        }

        public static DO.Volunteer addNewVolunteer(DO.Volunteer myVolunteer, BO.Volunteer v1)
        {
            var finalVolunteer = new DO.Volunteer
            {
                Id = myVolunteer.Id,
                FullName = myVolunteer.FullName,
                Email = myVolunteer.Email,
                PhoneNumber = myVolunteer.PhoneNumber,
                FullAddress = myVolunteer.FullAddress,
                Latitude = CallManager.GetLatitudLongitute(v1.Address).Latitude,
                Longitude = CallManager.GetLatitudLongitute(v1.Address).Longitude,
                Role = myVolunteer.Role
            };
            return finalVolunteer;
        }
  
        public static BO.CallInProgress addCall(DO.Assignment aInProgress,DO.Volunteer volunteer)
        {
            DO.Call call = s_dal.Call.Read(aInProgress.CallId);
            return new BO.CallInProgress
            {
                Id = aInProgress.Id,
                CallId = call.Id,
                CallType = (BO.CallType)call.CallType,
                CallAddress = call.FullAddressCall,
                CallDescription = call.VerbalDescription,
                MaxEndTime = call.MaxTimeFinish,
                TimeCallMade = call.openTime,
                EntryTimeTreatment = aInProgress.EnteryTimeTreatment,
                DistanceCallFromVolunteer = CalculateDistance(call.Latitude, call.Longitude, (double)volunteer.Latitude, (double)volunteer.Longitude),
                Status = (call.MaxTimeFinish.Value - DateTime.Now) <= AdminImplementation.GetRiskTimeRange()
    ? BO.Status.InRiskProgress
    : BO.Status.InProgress
            };
        }
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            
            double R = 6371; // רדיוס כדור הארץ בק\"מ
            double dLat = Math.PI / 180 * (lat2 - lat1);
            double dLon = Math.PI / 180 * (lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Math.PI / 180 * lat1) * Math.Cos(Math.PI / 180 * lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Validates and checks if the BO.Volunteer object can be updated.
        /// </summary>
        /// <param name="v1">The BO.Volunteer object to validate.</param>
        /// <param name="myStatus">Optional status to check against the volunteer's role.</param>
        /// <returns>True if validation passes, otherwise throws an exception.</returns>
        public static bool IntegrityChecker(BO.Volunteer v1)
        {
            if (v1 != null)
            {
                // Validate email
                if (!Helpers.VolunteerManager.isEmail(v1.Email))
                    throw new InvalidException("Invalid email");

                // Validate phone number
                if (!Helpers.VolunteerManager.isPhone(v1.Phone))
                    throw new InvalidException("Invalid phone");

                // Validate ID
                if (!Helpers.VolunteerManager.isId(v1.Id))
                    throw new InvalidException("Invalid id");

                // Validate address
                if (!Helpers.Tools.TryGetCoordinates(v1.Address, out var coordinates))
                    throw new InvalidException("Invalid address");
                
                return true;
            }
            return false;
        }
    }
}
