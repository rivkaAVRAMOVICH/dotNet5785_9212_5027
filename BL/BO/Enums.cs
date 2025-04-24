namespace BO;

    public enum Role { Manager, Volunteer }
    public enum DistanceType { Air, Walking, Driving }
    public enum CallType { Emergency,Regular,Consulation,Technical,None }
    public enum Status { Open,InProgress, InRiskProgress,Closed,Expired,openRisk,RiskOfExpiration,completed }
//public enum HandleCallType { None }
public enum EndCallType
{
    Treated,
    SelfCancellation,
    AdministratorCancellation,
    ExpiredCancellation,
    None
}
    public enum TimeUnit
    {
        MINUTE,
        HOUR,
        DAY,
        MONTH,
        SECOND,
        YEAR
    }
