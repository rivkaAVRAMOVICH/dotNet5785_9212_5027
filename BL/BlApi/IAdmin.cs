using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface IAdmin
    {
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5
        public DateTime GetClock();
        // מתודת קידום שעון
        public void AdvanceClock(BO.TimeUnit timeUnit);
        // מתודת בקשת טווח זמן סיכון
        public TimeSpan GetRiskTimeRange();

        void StartSimulator(int interval); //stage 7
        void StopSimulator(); //stage 7

        // מתודת הגדרת טווח זמן סיכון
        public void SetRiskTimeRange(TimeSpan riskTimeRange);


        // מתודת איפוס בסיס נתונים
        public void ResetDatabase();


        // מתודת אתחול בסיס נתונים
         public void InitializeDatabase();
       
    }
}

