using Interfaces;
using Microsoft.AspNetCore.Mvc;
using SeekerAPI.Services;

namespace SeekerAPI.controllers;

[ApiController]
[Route("[controller]")]
public class ServiceStateController(IServiceStateService _state) : Controller
{
    [HttpGet]
    public SyncState GetServiceSyncState()
    {
        return _state.GetSyncState();
    }
}