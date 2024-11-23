namespace Dal;

internal static class Config
{
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallNum { get => nextCallId++; }
    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
    
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}