using System.Text.RegularExpressions;
using Dapper;
using Interfaces;

namespace DAL;

public class FilterRepository
{
    private const string ADD_FILTER =
        """
        INSERT INTO filter(text, type, subtype)
        VALUES (@text, @type, @subtype)
        """;

    private const string GET_ALL_FILTERS =
        """
        SELECT id, text, type, subtype
        FROM filter
        """;

    public void AddFilter(string text, JobFilterType type, JobFilterSubtype subtype)
    {
        using var context = new DbContext();
        context.Connection.Execute(ADD_FILTER, new { text, type, subtype });
    }

    public IEnumerable<JobFilter> GetAllFilters()
    {
        using var context = new DbContext();
        return context.Connection.Query<JobFilter>(GET_ALL_FILTERS);
    }

    public void AddPointerToFilter(Job job, JobFilter filter)
    {
        switch (filter.Subtype)
        {
            case JobFilterSubtype.Content:
                job.Content = Regex.Replace(job.Content, filter.Text, $"<a class=\"filtered-text\">{filter.Text}</a>", RegexOptions.IgnoreCase);
                return;
            case JobFilterSubtype.Title:
                job.Preview = Regex.Replace(job.Preview, filter.Text, $"<a class=\\\"filtered-text\\\">{filter.Text}</a>", RegexOptions.IgnoreCase);
                return;
            default:
                return;
        }
    }
}