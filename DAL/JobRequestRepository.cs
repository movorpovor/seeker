using Dapper;
using Interfaces;

namespace DAL;

public class JobRequestRepository
{
    public Task<int> Insert(string text)
    {
        using var context = new DbContext();
        return context.Connection.ExecuteAsync(ADD_REQUEST, new { text });
    }

    public int SetJobToRequestRelation(int requestId, IEnumerable<int> jobIds)
    {
        using var context = new DbContext();
        return context.Connection.Execute(
            SET_JOB_REQUEST_RELATION,
            jobIds.Select(x => new { jobId = x, requestId })
        );
    }

    public int UpdateRequestDate(JobRequest request)
    {
        using var context = new DbContext();
        return context.Connection.Execute(UPDATE_REQUEST_DATE, request);
    }

    public IEnumerable<JobRequest> GetAllJobsRequestsInformation()
    {
        using var context = new DbContext();
        return context.Connection.Query<JobRequest>(GET_ALL_REQUESTS_INFORMATION);
    }

    #region QERIES

    private const string ADD_REQUEST =
        """
        INSERT INTO request (text)
        VALUES (@Text)
        """;

    private const string GET_NOT_UPDATED_REQUESTS =
        """
        SELECT id, text
        FROM request
        WHERE last_update_date IS NULL OR last_update_date < @date
        """;

    private const string SET_JOB_REQUEST_RELATION =
        """
        INSERT INTO job_to_request (job_id, request_id)
        VALUES(@jobId, @requestId)
        ON CONFLICT DO NOTHING
        """;

    private const string UPDATE_REQUEST_DATE =
        """
        UPDATE request
        SET last_update_date = @LastUpdateDate
        WHERE id = @Id
        """;

    private const string GET_ALL_REQUESTS_INFORMATION =
        """
        SELECT id, text, last_update_date as LastUpdateDate
        FROM request
        """;

    #endregion
}