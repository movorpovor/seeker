using Interfaces;

namespace SeekerAPI.Services;

public class ServiceStateService(IJobRepository jobRepository) : IServiceStateService
{
    private readonly ServiceState _state = new();
    private readonly IJobRepository _jobRepository = jobRepository;

    public void SetCurrentStateOfRequestSync(string request, int percentage)
    {
        _state.SyncState.CurrentRequest = request;
        _state.SyncState.ProgressPercentage = percentage;
        _state.SyncState.CurrentPage = 1;
        _state.SyncState.CurrentPagePercentage = 0;
    }

    public void SetCurrentPageState(int percentage, int page = -1)
    {
        _state.SyncState.CurrentPagePercentage = percentage;
        
        if (page > 0)
            _state.SyncState.CurrentPage = page;
    }

    public void SetCurrentSyncState(bool inProgress)
    {
        if (inProgress)
        {
            _state.SyncState = new SyncState()
            {
                InProgress = true,
                StartedAt = DateTime.Now
            };

            return;
        }
        
        _state.SyncState.InProgress = inProgress;
    }

    public bool DoesSyncCanBePerformed()
    {
        return !_state.SyncState.InProgress;
    }

    public SyncState GetSyncState() => _state.SyncState;

    public void AddJobs(int uniq, int important = 0, int ignored = 0)
    {
        _state.SyncState.UniqJobsCount += uniq;
        _state.SyncState.ImportantCount += important;
        _state.SyncState.AutoIgnoredCount += ignored;
    }
}