using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using DAL;
using Interfaces;

namespace SeekHandler;

public class JobRetriever(
    IJobRepository _jobRepository,
    JobRequestRepository _jobRequestRepository,
    FilterRepository _filterRepository,
    IServiceStateService _serviceState
)
{
    private const string DataStartFlag = "window.SEEK_REDUX_DATA = ";
    private const string DataEndFlag = "window.SEEK_APP_CONFIG";

    public async Task<int> UpdateAllRequests()
    {
        var requestList = _jobRequestRepository.GetAllJobsRequestsInformation().ToArray();
        var filters = _filterRepository.GetAllFilters().ToArray();
        Console.WriteLine($"found requests - {requestList.Length}");
        
        for (var i = 0; i < requestList.Length; i++)
        {
            Console.WriteLine($"[{requestList[i].Text}] starting handle request");

            _serviceState.SetCurrentStateOfRequestSync(requestList[i].Text, i * 100 / requestList.Length);
            await HandleJobRequest(requestList[i], filters);
        }
        
        _serviceState.SetCurrentSyncState(false);
        
        return 2;
    }

    private async Task<List<int>> HandleJobRequest(JobRequest request, JobFilter[] filters)
    {
        var page = 1;
        Console.WriteLine($"[{request.Text}] starting handling jobs. page = {page}");
        var jobNodes = await GetData(request.Text);
        var jobsList = new List<int>();
        
        while (jobNodes != null)
        {
            _serviceState.SetCurrentPageState(0, page);
            Console.WriteLine($"[{request.Text}] received jobs");
            var jobs =
                jobNodes
                    .AsArray()
                    .Select(x => new Job(x))
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .ToArray();
            
            Console.WriteLine($"[{request.Text}] jobs found - {jobs.Length}");

            var allJobsIds = jobs.Select(x => x.Id).ToArray();
            var duplicates = _jobRepository.GetDuplicates(jobs.Select(x => x.Id).ToArray()).ToArray();

            Console.WriteLine($"[{request.Text}] duplicates jobs - {duplicates.Length}");
            
            jobs = jobs.Where(x => !duplicates.Contains(x.Id)).ToArray();
            
            FillJobsContentAndFilterFromSeek(jobs, filters);

            Console.WriteLine($"[{request.Text}] start insert job list page {page} into DB");
        
            _serviceState.AddJobs(
                jobs.Length, 
                jobs.Count(x=>x.Filter == JobFilterType.Important),
                jobs.Count(x=>x.Filter == JobFilterType.AutoIgnore));
            
            _jobRepository.Insert(jobs);
            _jobRequestRepository.SetJobToRequestRelation(request.Id, allJobsIds);
            jobsList.AddRange(jobs.Select(x=>x.Id));

            Console.WriteLine($"[{request.Text}] jobs inserted - {jobs.Length}");
            
            Thread.Sleep(1 * 1000);
            
            Console.WriteLine($"[{request.Text}] starting handling jobs. page = {page + 1}");
            jobNodes = await GetData(request.Text, ++page);
        }

        return jobsList;
    }

    private void FillJobsContentAndFilterFromSeek(Job[] jobs, JobFilter[] filters)
    {
        for (var i = 0; i < jobs.Length; i++)
        {
            jobs[i].Content = GetDataForJob(jobs[i].Id).Result;
            Thread.Sleep(1 * 1000);
            Console.WriteLine($"job requested - {jobs[i].Id}");

            foreach (var filter in filters)
            {
                if (!jobs[i].Content.Contains(filter.Text)) 
                    continue;
                
                _filterRepository.AddPointerToFilter(jobs[i], filter.Text);

                jobs[i].Filter = filter.Type;
            }
            
            _serviceState.SetCurrentPageState(i * 100 / jobs.Length);
        }
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
    
    private async Task<string> GetDataForJob(int jobId)
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