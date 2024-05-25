namespace Interfaces;

public interface IJobRepository
{
    int Insert(Job[] jobs);
    
    IEnumerable<int> GetDuplicates(int[] ids);
    
    IEnumerable<Job> GetJobsDescriptionList(int limit, int offset, JobFilterType filter = JobFilterType.None);

    int GetJobsCountByFilter(JobFilterType filter);

    int SetFilterToJob(int jobId, JobFilterType filter);

    void FilterExistingJobs(string filterExpression, JobFilterType filter, JobFilterSubtype subtype);

    void CleanHiddenJobs();
}