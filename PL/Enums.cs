using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class Enums
    {
        internal class AssignmentCollection : IEnumerable
        {
            static readonly IEnumerable<BO.AssignmentSortField> s_enums =
                (System.Enum.GetValues(typeof(BO.AssignmentSortField)) as IEnumerable<BO.AssignmentSortField>)!;
            public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
        }

        internal class CallCollection : IEnumerable
        {
            static readonly IEnumerable<BO.CallSortField> s_enums =
                (System.Enum.GetValues(typeof(BO.CallSortField)) as IEnumerable<BO.CallSortField>)!;
            public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
        }

        internal class VolunteerCollection : IEnumerable
        {
            static readonly IEnumerable<BO.VolunteerSortField> s_enums =
                (System.Enum.GetValues(typeof(BO.VolunteerSortField)) as IEnumerable<BO.VolunteerSortField>)!;
            public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
        }
    }
}
