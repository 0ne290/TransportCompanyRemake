namespace Domain.Constants;

public class DriverWeeklyWorkloadStatuses
{
    public static bool IsStatus(string status) => ValidStatuses.Contains(status);
    
    public const string AwaitingAssignmentOfPerformers = "Awaiting assignment of performers";
    
    public const string PerformersAssigned = "Performers assigned";
    
    public const string InProgress = "In progress";
    
    public const string Completed = "Completed";

    private static readonly HashSet<string> ValidStatuses = new(new[]
        { AwaitingAssignmentOfPerformers, PerformersAssigned, InProgress, Completed });
}