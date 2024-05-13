using DAL;
using Interfaces;
using Microsoft.AspNetCore.Mvc;
using SeekerAPI.Services;
using SeekHandler;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class JobController(
    JobRepository _jobRepository, 
    IServiceScopeFactory _serviceScopeFactory,
    ServiceStateService _serviceStateService): Controller
{
    [HttpGet]
    public IEnumerable<Job> GetJobsList(int count, int offset, JobFilterType filter)
    {
        return _jobRepository.GetJobsDescriptionList(count, offset, filter);
    }
    
    [HttpPost]
    [Route("retrieveJobs")]
    public async void RetrieveJobsList()
    {
        if (_serviceStateService.State.RetrieveInProgress)
            return;
        
        _serviceStateService.State.RetrieveInProgress = true;
        
        var scope = _serviceScopeFactory.CreateScope();
        var newRetriever = scope.ServiceProvider.GetService<JobRetriever>();
        Task.Run(() =>
        {
            using (scope)
            {
                newRetriever.UpdateAllRequests().ContinueWith(task =>
                {
                    _serviceStateService.State.RetrieveInProgress = false; 
                });
            }
        });
    }
    
    [HttpPost]
    [Route("hide")]
    [ProducesResponseType(typeof(Job), 200)]
    [ProducesResponseType(204)]
    public Job? HideJob(int jobId, int pageLength, int page)
    {
        _jobRepository.SetFilterToJob(jobId, JobFilterType.Hidden);
        
        return _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault();
    }

    [HttpPost]
    [Route("apply")]
    [ProducesResponseType(typeof(Job), 200)]
    [ProducesResponseType(204)]
    public Job? ApplyForAJob(int jobId, int pageLength, int page)
    {
        _jobRepository.SetFilterToJob(jobId, JobFilterType.Applied);
        
        return _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault();
    }
}