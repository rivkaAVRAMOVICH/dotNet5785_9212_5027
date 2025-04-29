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
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock()
    {
        return _dal.Config.Clock;
    }
    // Clock promotion method
    public void AdvanceClock(BO.TimeUnit timeUnit) {
        switch (timeUnit)
        {
            case BO.TimeUnit.SECOND:
                ClockManager.UpdateClock(ClockManager.Now.AddSeconds(1));
                break;
            case BO.TimeUnit.MINUTE:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                break;
            case BO.TimeUnit.HOUR:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                break;
            case BO.TimeUnit.DAY:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                break;
            case BO.TimeUnit.MONTH:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
                break;
            case BO.TimeUnit.YEAR:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
                break;
        }
    }
    // Risk Time Range Request Method
    public TimeSpan GetRiskTimeRange()
{
        return _dal.Config.RiskRange;
    }

    // Risk time frame definition method
    public void SetRiskTimeRange(TimeSpan riskTimeRange) {
        _dal.Config.RiskRange = riskTimeRange;
    }

    // Database reset method
    public void ResetDatabase()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    // Database initialization method
    public void InitializeDatabase() {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}
