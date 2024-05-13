using DAL;
using Interfaces;
using SeekerAPI.Models;

namespace SeekerAPI.Services;

public class ServiceStateService
{
    public ServiceState State { get; }
    private readonly JobRepository _jobRepository;

    public ServiceStateService(JobRepository jobRepository)
    {
        _jobRepository = jobRepository;
        State = new ServiceState()
        {
            JobsCount = _jobRepository.GetJobsCountByFilter(JobFilterType.None)
        };
    }
}