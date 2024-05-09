using Dapper;
using Interfaces;

namespace DAL;

public class JobRepository
{
    private const string INSERT_JOB =
        """
        INSERT INTO job (id, preview, posted_date, content)
        VALUES (@Id, @Preview::json, @PostedDate, @Content)
        """;

    private const string DUPLICATE_IDS =
        """
        SELECT id
        FROM job
        WHERE id = ANY(@ids)
        """;

    private const string GET_JOBS_DESCRIPTION =
        """
        SELECT *
        FROM job
        WHERE id NOT IN (
            SELECT job_id
            FROM hidden_jobs
            
            UNION 
            
            SELECT job_id
            FROM applied_jobs
        )
        ORDER BY posted_date
        LIMIT @limit OFFSET @offset
        """;

    private const string GET_JOBS_COUNT =
        """
        SELECT COUNT(1)
        FROM job
        WHERE id NOT IN (
            SELECT job_id
            FROM hidden_jobs
            
            UNION 
            
            SELECT job_id
            FROM applied_jobs
        )
        """;

    private const string HIDE_JOB =
        """
        INSERT INTO hidden_jobs
        VALUES (@jobId)
        """;

    private const string MARK_JOB_AS_APPLIED =
        """
        INSERT INTO applied_jobs
        VALUES(@jobId, @appliedDate)
        """;
    
    private const string GET_APPLIED_JOBS_DESCRIPTION =
        """
        SELECT *
        FROM job j RIGHT JOIN applied_jobs hj ON j.id = hj.job_id
        """;
    
    private const string GET_HIDDEN_JOBS_DESCRIPTION =
        """
        SELECT *
        FROM job j RIGHT JOIN hidden_jobs hj ON j.id = hj.job_id
        """;
    
    public Task<int> InsertAsync(Job job)
    {
        using var context = new DbContext();
        return context.Connection.ExecuteAsync(INSERT_JOB, job);
    }
    
    public int Insert(Job[] jobs)
    {
        Console.WriteLine($"insert jobs - {jobs.Length}");
        if (jobs.Length == 0) return 0;
        
        using var context = new DbContext();
        return context.Connection.Execute(INSERT_JOB, jobs);
    }

    public IEnumerable<int> GetDuplicates(int[] ids)
    {
        Console.WriteLine($"fetching duplicates");
        using var context = new DbContext();
        return context.Connection.Query<int>(DUPLICATE_IDS, new { ids });
    }

    public IEnumerable<Job> GetJobsDescriptionList(int limit, int offset)
    {
        using var context = new DbContext();
        return context.Connection.Query<Job>(GET_JOBS_DESCRIPTION, new { limit, offset });
    }

    public int GetJobsCount()
    {
        using var context = new DbContext();
        return context.Connection.QueryFirst<int>(GET_JOBS_COUNT);
    }

    public int HideJob(int jobId)
    {
        using var context = new DbContext();
        return context.Connection.Execute(HIDE_JOB, new { jobId });
    }

    public void MarkJobAsApplied(int jobId, DateTime appliedDate)
    {
        using var context = new DbContext();
        context.Connection.Execute(MARK_JOB_AS_APPLIED, new { jobId, appliedDate });
    }

    public IEnumerable<Job> GetAppliedJobs(int limit, int offset)
    {
        using var context = new DbContext();
        return context.Connection.Query<Job>(GET_APPLIED_JOBS_DESCRIPTION, new { limit, offset });
    }
    
    public IEnumerable<Job> GetHiddenJobs(int limit, int offset)
    {
        using var context = new DbContext();
        return context.Connection.Query<Job>(GET_HIDDEN_JOBS_DESCRIPTION, new { limit, offset });
    }
}