using Microsoft.AspNetCore.Mvc;
using SeekerAPI.Models;
using SeekerAPI.Services;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class ServiceStateController(ServiceStateService _state) : Controller
{
    [HttpGet]
    public ServiceState GetServiceState()
    {
        return _state.State;
    }
}