using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IBl
    {
        IVolunteer Volunteer { get; }
        ICall Call { get; }
        IAdmin Admin { get; }
    }
}
