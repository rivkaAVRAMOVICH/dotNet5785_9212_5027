namespace DalApi;

public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }
    int MaxRange { get; }

    void Reset();
    void SetMaxRange(int value);
}