using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager
{
    #region DAL + Events
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    private static event Action? _configUpdatedObservers;
    private static event Action? _clockUpdatedObservers;

    internal static event Action? ConfigUpdatedObservers
    {
        add => _configUpdatedObservers += value;
        remove => _configUpdatedObservers -= value;
    }

    internal static event Action? ClockUpdatedObservers
    {
        add => _clockUpdatedObservers += value;
        remove => _clockUpdatedObservers -= value;
    }
    #endregion

    #region Properties
    // החליפי את MaxRange במשתנה הסביבה הרלוונטי אצלך אם צריך
    internal static int MaxRange
    {
        get => s_dal.Config.MaxRange;
        set
        {
            s_dal.Config.SetMaxRange(value); // אם אין לך את זה, תורידי את השורה או תשני בהתאם
            s_dal.Config.Reset();
            _configUpdatedObservers?.Invoke();
        }
    }

    internal static DateTime Now => s_dal.Config.Clock;
    #endregion

    #region StudentManager (יש להגדיר מהצד שלך)
    public static dynamic StudentManager { get; set; } = default!;
    #endregion

    #region Reset / Init
    internal static void ResetDB()
    {
        lock (BlMutex)
        {
            s_dal.ResetDB();
            UpdateClock(Now);
            MaxRange = MaxRange;
        }
    }

    internal static void InitializeDB()
    {
        lock (BlMutex)
        {
            DalTest.Initialization.Do();
            UpdateClock(Now);
            MaxRange = MaxRange;
        }
    }
    #endregion

    #region UpdateClock
    private static Task? _periodicTask = null;

    internal static void UpdateClock(DateTime newClock)
    {
        var oldClock = s_dal.Config.Clock;
        s_dal.Config.Clock = newClock;

        if (_periodicTask is null || _periodicTask.IsCompleted)
        {
            _periodicTask = Task.Run(() =>
            {
                StudentManager?.PeriodicStudentsUpdates(oldClock, newClock);
            });
        }

        _clockUpdatedObservers?.Invoke();
    }
    #endregion

    #region סימולטור - שלב 7

    internal static readonly object BlMutex = new();
    private static volatile Thread? s_thread;
    private static volatile bool s_stop = false;
    private static int s_interval = 1;
    private static Task? _simulateTask = null;

    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
            throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new Thread(ClockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt();
            s_thread = null;
        }
    }

    private static void ClockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            if (_simulateTask is null || _simulateTask.IsCompleted)
            {
                _simulateTask = Task.Run(() =>
                {
                    StudentManager?.SimulateCourseRegistrationAndGrade();
                });
            }

            try
            {
                Thread.Sleep(1000);
            }
            catch (ThreadInterruptedException) { }
        }
    }
    #endregion
}
