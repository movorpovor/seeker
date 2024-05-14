using Dapper;
using Interfaces;

namespace DAL;

public class JobRepository
{
    private const string INSERT_JOB =
        """
        INSERT INTO job (id, preview, posted_date, content, filter)
        VALUES (@Id, @Preview::json, @PostedDate, @Content, @Filter)
        """;

    private const string DUPLICATE_IDS =
        """
        SELECT id
        FROM job
        WHERE id = ANY(@ids)
        """;

    private const string GET_JOBS_DESCRIPTION_WITH_FILTER =
        """
        SELECT *
        FROM job
        WHERE filter = @filter
        ORDER BY posted_date
        LIMIT @limit OFFSET @offset
        """;

    private const string GET_JOBS_COUNT =
        """
        SELECT COUNT(1)
        FROM job
        WHERE filter = @filter
        """;

    private const string SET_FILTER_TO_JOB =
        """
        UPDATE Job
        SET filter = @filter
        WHERE id = @jobId
        """;
    
    private const string SET_FILTER_BY_CONTENT =
        """
        UPDATE job
        SET filter = @filter, 
            content = regexp_replace(content, @filterExpression, @filterToReplace, 'i')
        WHERE content ilike @filterExpression and filter <> @hiddenFilter;
        """;
    
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

    public IEnumerable<Job> GetJobsDescriptionList(int limit, int offset, JobFilterType filter = JobFilterType.None)
    {
        using var context = new DbContext();
        return context.Connection.Query<Job>(
            GET_JOBS_DESCRIPTION_WITH_FILTER, 
            new { limit, offset, filter }
        );
    }

    public int GetJobsCountByFilter(JobFilterType filter)
    {
        using var context = new DbContext();
        return context.Connection.QueryFirst<int>(
            GET_JOBS_COUNT,
            new { filter }    
        );
    }

    public int SetFilterToJob(int jobId, JobFilterType filter)
    {
        using var context = new DbContext();
        return context.Connection.Execute(
            SET_FILTER_TO_JOB, 
            new { filter, jobId }
        );
    }

    public void FilterExistingJobs(string filterExpression, JobFilterType filter)
    {
        using var context = new DbContext();
        context.Connection.Execute(
            SET_FILTER_BY_CONTENT, 
            new
            {
                filter,
                filterExpression = $"%{filterExpression}%",
                hiddenFilter = JobFilterType.Hidden,
                filterToReplace = $"<a class=\"filtered-text\">{filterExpression}</a>"
            });
    }
}