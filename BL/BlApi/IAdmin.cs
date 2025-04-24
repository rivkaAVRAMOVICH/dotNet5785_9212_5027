using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IAdmin
    {
        public DateTime GetClock();
        // מתודת קידום שעון
        public void AdvanceClock(BO.TimeUnit timeUnit);
        // מתודת בקשת טווח זמן סיכון
        public TimeSpan GetRiskTimeRange();


        // מתודת הגדרת טווח זמן סיכון
        public void SetRiskTimeRange(TimeSpan riskTimeRange);


        // מתודת איפוס בסיס נתונים
        public void ResetDatabase();


        // מתודת אתחול בסיס נתונים
         public void InitializeDatabase();
       
    }
}

