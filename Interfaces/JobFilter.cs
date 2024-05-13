namespace Interfaces;

public enum JobFilterType 
{   
    None,
    Important,
    AutoIgnore,
    Hidden,
    Applied
}

public class JobFilter
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public JobFilterType Type { get; set; }
}