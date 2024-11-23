namespace Dal;

public class ConfigImplementation : Iconfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public void Reset()
    {
        Config.Reset();
    }

}
