using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock()
    {
        return AdminManager.Clock;
    }
    // Clock promotion method
    public void AdvanceClock(BO.TimeUnit timeUnit) {
        AdminManager.ThrowOnSimulatorIsRunning();
        switch (timeUnit)
        {
            case BO.TimeUnit.SECOND:
                AdminManager.UpdateClock(AdminManager.Clock.AddSeconds(1));
                break;
            case BO.TimeUnit.MINUTE:
                AdminManager.UpdateClock(AdminManager.Clock.AddMinutes(1));
                break;
            case BO.TimeUnit.HOUR:
                AdminManager.UpdateClock(AdminManager.Clock.AddHours(1));
                break;
            case BO.TimeUnit.DAY:
                AdminManager.UpdateClock(AdminManager.Clock.AddDays(1));
                break;
            case BO.TimeUnit.MONTH:
                AdminManager.UpdateClock(AdminManager.Clock.AddMonths(1));
                break;
            case BO.TimeUnit.YEAR:
                AdminManager.UpdateClock(AdminManager.Clock.AddYears(1));
                break;
        }
    }
    // Risk Time Range Request Method
    public TimeSpan GetRiskTimeRange()
{
        return AdminManager.RiskRange;
    }

    // Risk time frame definition method
    public void SetRiskTimeRange(TimeSpan riskTimeRange) {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.RiskRange = riskTimeRange;
    }

    // Database reset method
    public void ResetDatabase()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        lock (AdminManager.BlMutex) //stage 7
            _dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Clock);
    }

    // Database initialization method
    public void InitializeDatabase() {
        AdminManager.ThrowOnSimulatorIsRunning();
        DalTest.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Clock);
    }
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
public void StopSimulator()
    => AdminManager.Stop(); //stage 7
}
