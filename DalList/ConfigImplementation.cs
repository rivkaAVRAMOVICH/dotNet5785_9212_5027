namespace Dal;
using DalApi;
using DO;

public class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    TimeSpan IConfig.RiskRange { get => Config.RiskRange; set => Config.RiskRange = value; }

    public void Reset()
    {
        Config.Reset();
    }
}