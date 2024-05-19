namespace Interfaces;

public interface IServiceStateService
{
    void SetCurrentStateOfRequestSync(string request, int percentage);
    void SetCurrentPageState(int percentage, int page = -1);
    void SetCurrentSyncState(bool inProgress);
    bool DoesSyncCanBePerformed();
    SyncState GetSyncState();
    void AddJobs(int uniq, int important = 0, int ignored = 0);
}