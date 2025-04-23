using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IAdmin
    {
       DateTime GetClock();
        // מתודת קידום שעון
         void AdvanceClock(BO.TimeUnit timeUnit);
        // מתודת בקשת טווח זמן סיכון
        TimeSpan GetRiskTimeRange();


        // מתודת הגדרת טווח זמן סיכון
         void SetRiskTimeRange(TimeSpan riskTimeRange);


        // מתודת איפוס בסיס נתונים
        void ResetDatabase();


        // מתודת אתחול בסיס נתונים
         void InitializeDatabase();
       
    }
}

