using DAL;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class FilterController(FilterRepository _filterRepository, JobRepository _jobRepository): Controller
{
    [HttpPost]
    [Route("addFilter")]
    public void AddFilter(string text, JobFilterType type)
    {
        _filterRepository.AddFilter(text, type);
        _jobRepository.FilterExistingJobs(text, type);
    }

    [HttpGet]
    [Route("getAllFilters")]
    public IEnumerable<JobFilter> GetAllFilters()
    {
        return _filterRepository.GetAllFilters();
    }
}