using Interfaces;
using Microsoft.AspNetCore.Mvc;
using SeekerAPI.Models;
using SeekerAPI.Services;
using SeekHandler;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class JobController(
    IJobRepository _jobRepository, 
    IServiceScopeFactory _serviceScopeFactory,
    IServiceStateService _serviceStateService): Controller
{
    [HttpGet]
    public JobRequestResponse JobsList(int count, int offset, JobFilterType filter)
    {

        return new JobRequestResponse
        {
            Jobs = _jobRepository.GetJobsDescriptionList(count, offset, filter),
            FullCount = _jobRepository.GetJobsCountByFilter(filter)
        };
    }
    
    [HttpPost]
    [Route("retrieveJobs")]
    public async void RetrieveJobsList()
    {
        if (!_serviceStateService.DoesSyncCanBePerformed())
            return;

        _serviceStateService.SetCurrentSyncState(true);

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
    public JobMoveResponse HideJob(int jobId, int pageLength, int page, JobFilterType filter)
    {
        _jobRepository.SetFilterToJob(jobId, JobFilterType.Hidden);

        return new JobMoveResponse
        {
            Job = _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault(),
            FullCount = _jobRepository.GetJobsCountByFilter(filter)
        };
    }

    [HttpPost]
    [Route("apply")]
    public JobMoveResponse ApplyForAJob(int jobId, int pageLength, int page, JobFilterType filter)
    {
        _jobRepository.SetFilterToJob(jobId, JobFilterType.Applied);
        
        return new JobMoveResponse
        {
            Job = _jobRepository.GetJobsDescriptionList(1, (page * pageLength) - 1).FirstOrDefault(),
            FullCount = _jobRepository.GetJobsCountByFilter(filter)
        };
    }
}