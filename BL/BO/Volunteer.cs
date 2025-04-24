using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Volunteer
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Role Role { get; set; }
        public Boolean IsActive { get; set; }
        public double? MaxDistance { get; set; }
        public TypeOfDistance TypeOfDistance { get; set; }
        public int SumHandledCalls { get; set; }
        public int SumCanceledCalls { get; set; }
        public int SumChosenExpiredCalls { get; set; }
        public BO.CallInProgress CallInVolunteerHandle { get; set; }
    }
}
