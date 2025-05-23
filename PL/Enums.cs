using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
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
        internal class RolesEnumCollection : IEnumerable
        {
            static readonly IEnumerable<BO.Role> s_positions =
                (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

            public IEnumerator GetEnumerator() => s_positions.GetEnumerator();
        }

        /// <summary>
        /// Collection for DistanceTypeEnum values, used in ComboBox binding for distance preferences
        /// </summary>
        internal class DistanceTypeEnumCollection : IEnumerable
        {
            static readonly IEnumerable<BO.TypeOfDistance> s_distanceTypes =
                (Enum.GetValues(typeof(BO.TypeOfDistance)) as IEnumerable<BO.TypeOfDistance>)!;

            public IEnumerator GetEnumerator() => s_distanceTypes.GetEnumerator();
        }
}
