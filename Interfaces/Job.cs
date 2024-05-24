using System.Text.Json.Nodes;

namespace Interfaces;

/// <summary>
///     Idea is that all data from seek stored inside JobData.
///     All other properties are stored for quick search and access.
/// </summary>
public class Job
{
    public int Id { get; set; }

    public string Preview { get; set; }
    
    public string Content { get; set; }
    
    public string Title { get; set; }
    
    public DateTime PostedDate { get; set; }
    
    public JobFilterType Filter { get; set; }

    public Job()
    {
        
    }
    
    public Job(JsonNode jobNode)
    {
        Id = jobNode["id"].GetValue<int>();
        PostedDate = jobNode["listingDate"].GetValue<DateTime>();
        Title = jobNode["title"].GetValue<string>();
        Preview = jobNode.ToJsonString();
        Filter = JobFilterType.None;
    }
}