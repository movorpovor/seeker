using Interfaces;

namespace SeekerAPI.Models;

public class JobMoveResponse
{
    public Job? Job { get; set; }
    public int FullCount { get; set; }
}