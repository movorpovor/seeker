using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using DAL;
using Interfaces;

namespace SeekHandler;

public class InsertJobResult(IEnumerable<int> insertedJobs, Dictionary<JobFilterType, List<int>> filteredJobs)
{
    public IEnumerable<int> InsertedJobs { get; set; } = insertedJobs;
    public Dictionary<JobFilterType, List<int>> FilteredJobs { get; set; } = filteredJobs;
}

public class JobRetriever(
    JobRepository _jobRepository,
    JobRequestRepository _jobRequestRepository,
    FilterRepository _filterRepository
)
{
    private const string DataStartFlag = "window.SEEK_REDUX_DATA = ";
    private const string DataEndFlag = "window.SEEK_APP_CONFIG";

    public async Task<int> UpdateAllRequests()
    {
        var requestList = _jobRequestRepository.GetAllJobsRequestsInformation();
        var filters = _filterRepository.GetAllFilters();
        Console.WriteLine($"found requests - {requestList.Count()}");
        foreach (var request in requestList)
        {
            Console.WriteLine($"starting handle request - {request}");
            var jobsIds = await RetrieveJobList(request.Text, filters);
            request.LastUpdateDate = DateTime.UtcNow;
            
            Console.WriteLine($"jobs fetch for request - {request}");
            Console.WriteLine($"setting relations for request - {request}");
            _jobRequestRepository.SetJobToRequestRelation(request.Id, jobsIds);
            Console.WriteLine($"updating request date for request - {request}");
            _jobRequestRepository.UpdateRequestDate(request);
        }

        return 2;
    }

    private async Task<IEnumerable<int>> RetrieveJobList(string request, IEnumerable<JobFilter> filters)
    {
        var page = 1;
        Console.WriteLine($"starting fetching jobs for {request} - {page}");
        var jobList = new Dictionary<int, Job>();
        var jobNodes = await GetData(request);
        
        while (jobNodes != null)
        {
            Console.WriteLine($"received jobs");
            foreach (var node in jobNodes.AsArray())
            {
                var job = new Job(node);
                jobList.TryAdd(job.Id, job);
            }

            Thread.Sleep(1 * 1000);

            Console.WriteLine($"starting fetching jobs for {request} - {page + 1}");
            jobNodes = await GetData(request, ++page);
        }

        Console.WriteLine($"All jobs handled {jobList.Count}");
        var insertedResult = InsertJobsIntoDb(jobList.Values.ToArray(), filters);
        
        return insertedResult.InsertedJobs;
    }

    private InsertJobResult InsertJobsIntoDb(Job[] values, IEnumerable<JobFilter> filters)
    {
        Console.WriteLine($"start insert job list into DB");
        var duplicates = _jobRepository.GetDuplicates(values.Select(x => x.Id).ToArray());
        Console.WriteLine($"duplicates found - {duplicates.Count()}");
        var newJobs = values.Where(x => !duplicates.Contains(x.Id)).ToArray();
        var filteredJobs = new Dictionary<JobFilterType, List<int>>();

        foreach (var job in newJobs)
        {
            job.Content = GetDataForJob(job.Id).Result;
            Thread.Sleep(1 * 1000);
            Console.WriteLine($"job requested - {job.Id}");

            foreach (var filter in filters)
            {
                if (!job.Content.Contains(filter.Text)) 
                    continue;
                
                _filterRepository.AddPointerToFilter(job, filter.Text);
                
                if (!filteredJobs.ContainsKey(filter.Type))
                    filteredJobs.Add(filter.Type, []);

                job.Filter = filter.Type;
                filteredJobs[filter.Type].Add(job.Id);
            }
        }
        
        _jobRepository.Insert(newJobs);

        Console.WriteLine($"jobs inserted - {newJobs.Length}");
        return new InsertJobResult(newJobs.Select(x => x.Id), filteredJobs);
    }

    private async Task<JsonNode> GetData(string request, int page = 1)
    {
        try
        {
            request = HttpUtility.UrlEncode(request.Replace(' ', '-'));
            var client = new HttpClient();
            var response =
                await client.GetAsync($"https://www.seek.co.nz/{request}-jobs/in-All-New-Zealand?daterange=2&page={page}");
            
            var result = await response.Content.ReadAsStringAsync();
            var dataStart = result.IndexOf(DataStartFlag) + DataStartFlag.Length;
            var dataEnd = result.IndexOf(DataEndFlag) - 4;
            result = result.Substring(dataStart, dataEnd - dataStart).Replace(":undefined", ":null");

            return JsonSerializer.Deserialize<JsonNode>(result)["results"]["results"]["jobs"];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<string> GetDataForJob(int jobId)
    {
        try
        {
            var client = new HttpClient();
            var response =
                await client.GetAsync($"https://www.seek.co.nz/job/{jobId}");
            
            var result = await response.Content.ReadAsStringAsync();
            var dataStart = result.IndexOf(DataStartFlag) + DataStartFlag.Length;
            var dataEnd = result.IndexOf(DataEndFlag) - 4;
            
            result = result.Substring(dataStart, dataEnd - dataStart).Replace(":undefined", ":null");

            var deserialized = JsonSerializer.Deserialize<JsonNode>(result);
            return deserialized["jobdetails"]["result"]["job"]["content"].GetValue<string>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}