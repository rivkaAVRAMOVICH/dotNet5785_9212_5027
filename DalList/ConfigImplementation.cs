namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    TimeSpan IConfig.RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
    {
        Config.Reset();
    }
}