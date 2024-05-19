namespace Interfaces;

public class SyncState
{
    public bool InProgress { get; set; }
    public string CurrentRequest { get; set; }
    public int ProgressPercentage { get; set; }
    public int CurrentPage { get; set; }
    public int CurrentPagePercentage { get; set; }
    public DateTime StartedAt { get; set; }
    public int UniqJobsCount { get; set; }
    public int ImportantCount { get; set; }
    public int AutoIgnoredCount { get; set; }
}