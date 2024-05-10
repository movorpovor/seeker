namespace Interfaces;

public enum JobFilterType 
{
    Important,
    FilterOut
}

public class JobFilter
{
    public int Id { get; set; }
    
    public string Text { get; set; }
    
    public JobFilterType Type { get; set; }
}