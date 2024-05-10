using Dapper;
using Interfaces;

namespace DAL;

public class FilterRepository
{
    private const string ADD_FILTER =
        """
        INSERT INTO filter(filter_text, filter_type)
        VALUES (@text, @type)
        """;

    private const string GET_ALL_FILTERS =
        """
        SELECT id, filter_text as text, filter_type as type
        FROM filter
        """;

    public void AddFilter(string text, JobFilterType type)
    {
        using var context = new DbContext();
        context.Connection.Execute(ADD_FILTER, new { text, type });
    }

    public IEnumerable<JobFilter> GetAllFilters()
    {
        using var context = new DbContext();
        return context.Connection.Query<JobFilter>(GET_ALL_FILTERS);
    }
}