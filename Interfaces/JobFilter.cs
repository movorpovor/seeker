namespace Interfaces;

public enum JobFilterType 
{   
    None,
    Important,
    AutoIgnore,
    Hidden,
    Applied
}

public enum JobFilterSubtype
{
    Content,
    Title
}

public class JobFilter
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public JobFilterType Type { get; set; }
    
    public JobFilterSubtype Subtype { get; set; }
}