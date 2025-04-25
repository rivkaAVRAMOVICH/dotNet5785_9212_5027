using BlApi;
using Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation
{
    internal class Bl : IBl
    {
        public ICall Call { get; } = new CallImplementation();
        public IVolunteer Volunteer { get; } = new VolunteerImplementation();
        public IAdmin Admin { get; } = new AdminImplementation();
    }
}
