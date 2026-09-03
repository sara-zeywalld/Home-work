using backend.MetricsBL.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/premium-methods")]
public class PremiumMethodsController(IPremiumMethodService premium_method_service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await premium_method_service.GetAsync());

    [HttpPost]
    public async Task<IActionResult> Create(PremiumMethod method)
    {
        var result = await premium_method_service.CreateAsync(method);
        return result.IsValid ? Created($"api/premium-methods/{method.Id}", method) : BadRequest("Number and description are required.");
    }
}
