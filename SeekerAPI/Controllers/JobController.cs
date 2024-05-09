using System.Formats.Asn1;
using System.Text.Json.Nodes;
using DAL;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using SeekHandler;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class JobController(JobRepository _jobRepository, IServiceScopeFactory _serviceScopeFactory): Controller
{
    [HttpGet]
    public IEnumerable<Job> GetJobsList(int count, int offset)
    {
        return _jobRepository.GetJobsDescriptionList(count, offset);
    }
    
    [HttpPost]
    [Route("retrieveJobs")]
    public async void RetrieveJobsList()
    { 
        var scope = _serviceScopeFactory.CreateScope();
        var newRetriever = scope.ServiceProvider.GetService<JobRetriever>();
        Task.Run(() =>
        {
            using (scope)
            {
                newRetriever.UpdateAllRequests();
            }
        });
    }
    
    [HttpPost]
    [Route("hide")]
    [ProducesResponseType(typeof(Job), 200)]
    [ProducesResponseType(204)]
    public Job? HideJob(int jobId, int pageLength, int page)
    {
        _jobRepository.HideJob(jobId);
        
        return _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault();
    }

    [HttpPost]
    [Route("apply")]
    [ProducesResponseType(typeof(Job), 200)]
    [ProducesResponseType(204)]
    public Job? ApplyForAJob(int jobId, int pageLength, int page)
    {
        _jobRepository.MarkJobAsApplied(jobId, DateTime.UtcNow);
        
        return _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault();
    }
    
    [HttpGet]
    [Route("getHiddenJobs")]
    public IEnumerable<Job> GetHiddenJobsList(int count, int offset)
    {
        return _jobRepository.GetHiddenJobs(count, offset);
    }
    
    [HttpGet]
    [Route("getAppliedJobs")]
    public IEnumerable<Job> GetAppliedJobsList(int count, int offset)
    {
        return _jobRepository.GetAppliedJobs(count, offset);
    }
}