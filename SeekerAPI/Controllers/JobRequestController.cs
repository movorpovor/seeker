using DAL;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class JobRequestController(JobRequestRepository _jobRequestRepository)
{
    [HttpGet]
    public IEnumerable<JobRequest> GetAllJobRequests()
    {
        return _jobRequestRepository.GetAllJobsRequestsInformation();;
    }

    [HttpPost]
    [Route("/insertRequest")]
    public void InsertNewRequest(string request)
    {
        _jobRequestRepository.Insert(request);
    }
}