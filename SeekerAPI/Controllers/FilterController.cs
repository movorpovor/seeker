using DAL;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class FilterController(FilterRepository _filterRepository, IJobRepository _jobRepository): Controller
{
    [HttpPost]
    [Route("addFilter")]
    public void AddFilter(string text, JobFilterType type, JobFilterSubtype subtype)
    {
        _filterRepository.AddFilter(text, type, subtype);
        _jobRepository.FilterExistingJobs(text, type, subtype);
    }

    [HttpGet]
    [Route("getAllFilters")]
    public IEnumerable<JobFilter> GetAllFilters()
    {
        return _filterRepository.GetAllFilters();
    }
}