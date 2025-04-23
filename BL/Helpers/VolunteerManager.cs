//using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
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
                Name = volunteer.Name,
                Active = volunteer.Active,
                SumOfCalls = volunteer.SumOfCalls,
                SumOfCanceledCalls = volunteer.SumOfCanceledCalls,
                SumOfExpiredCalls = volunteer.SumOfExpiredCalls,
                IdCallInProgress = null,
                TypeCallInProgress = volunteer.callInProgress != null
                    ? (BO.CallType)volunteer.callInProgress // Conversion
                    : BO.CallType.none
            });
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
                SumHandledCalls = tmpVolunteer.SumOfCalls,
                SumCanceledCalls = tmpVolunteer.SumOfCanceledCalls,
                SumChosenExpiredCalls = tmpVolunteer.SumOfExpiredCalls,
                CallInVolunteerHandle = (BO.CallInProgress)tmpVolunteer.callInProgress
            };
            return final;
        }

        /// <summary>
        /// Creates a new DO.Volunteer object based on a BO.Volunteer object.
        /// </summary>
        /// <param name="myVolunteer">Original DO.Volunteer object.</param>
        /// <param name="v1">BO.Volunteer object with updated information.</param>
        /// <returns>A DO.Volunteer object.</returns>
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
        +
        /// <summary>
        /// Adds a new call to a BO.Volunteer object.
        /// </summary>
        /// <param name="callInProgress">DO.Call object representing the ongoing call.</param>
        /// <param name="final">BO.Volunteer object to update.</param>
        /// <returns>An updated BO.Volunteer object with the new call information.</returns>
        public static BO.Volunteer addNewCall(DO.Call callInProgress, BO.Volunteer final)
        {
            double latV = final.Latitude.Value;
            double longV = final.Longitude.Value;

            // Add an object representing the ongoing call
            final.CallInVolunteerHandle = new BO.CallInProgress
            {
                Id = callInProgress.Id,
                CallId = callInProgress.Id,
                CallType = (BO.CallType)callInProgress.CallType,
                CallDescription = callInProgress.VerbalDescription,
                CallAddress = callInProgress.FullAddressCall,
                TimeCallMade = callInProgress.openTime,
                MaxEndTime = callInProgress.MaxTimeFinish,
                //StartTime = DateTime.Now,
                DistanceCallFromVolunteer = CallManager.GetDistance(latV, longV, callInProgress.Latitude, callInProgress.Longitude),
                Status =BO.Status.open
            };
            return final;
        }


        /// <summary>
        /// Validates and checks if the BO.Volunteer object can be updated.
        /// </summary>
        /// <param name="v1">The BO.Volunteer object to validate.</param>
        /// <param name="myStatus">Optional status to check against the volunteer's role.</param>
        /// <returns>True if validation passes, otherwise throws an exception.</returns>
        public static bool checkedToUpdate(BO.Volunteer v1, BO.Role? myStatus)
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

                // Check if the role was changed
                bool roleChanged = myStatus.HasValue && myStatus != v1.Role;

                // Ensure only volunteers or managers can update
                if (myStatus != BO.Role.Volunteer && roleChanged)
                {
                    throw new InvalidException("Only the volunteer or a manager can update this information.");
                }
                return true;
            }
            return false;
        }
    }
}
