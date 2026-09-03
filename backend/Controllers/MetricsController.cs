using backend.MetricsBL.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/metrics")]
public class MetricsController(IMetricService metric_service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMetrics() => Ok(await metric_service.GetMetricsAsync());

    [HttpPost]
    public async Task<IActionResult> Create(Metric metric)
    {
        if (metric.PremiumMethodId <= 0 || string.IsNullOrWhiteSpace(metric.Code) || string.IsNullOrWhiteSpace(metric.Name))
            return BadRequest("Premium method, code, and name are required.");

        return Ok(await metric_service.CreateAsync(metric));
    }

    [HttpGet("{id}/rows")]
    public async Task<IActionResult> GetRows(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? search = null)
    {
        var result = await metric_service.GetRowsAsync(id, page, pageSize, search);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/imports")]
    public async Task<IActionResult> GetImports(int id) => Ok(await metric_service.GetImportsAsync(id));

    [HttpPost("{id}/import")]
    public async Task<IActionResult> Import(int id, IFormFile file, [FromForm] int year, [FromForm] string period)
    {
        if (file is null) return BadRequest("A file is required.");
        if (file.Length == 0) return BadRequest("The file is empty.");
        var extension = Path.GetExtension(file.FileName);
        if (!new[] { ".csv", ".xlsx", ".xls" }.Contains(extension, StringComparer.OrdinalIgnoreCase)) return BadRequest("Only CSV and Excel files are supported.");
        var result = await metric_service.ImportAsync(id, file.FileName, year, period, file.OpenReadStream());
        return result is null ? NotFound() : Ok(result);
    }
}
