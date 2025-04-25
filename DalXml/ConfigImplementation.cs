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
    TimeSpan IConfig.RiskRange { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public void Reset()
    {
        Config.Reset();
    }
}