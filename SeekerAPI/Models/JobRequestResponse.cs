using Interfaces;

namespace SeekerAPI.Models;

public class JobRequestResponse
{
    public IEnumerable<Job> Jobs { get; set; }
    public int FullCount { get; set; }
}