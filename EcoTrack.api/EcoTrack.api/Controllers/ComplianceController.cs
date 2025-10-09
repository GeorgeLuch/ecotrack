using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/compliance")]
public class ComplianceController : ControllerBase
{
    private readonly ComplianceService _service;
    public ComplianceController(ComplianceService service) => _service = service;

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] Norma norma)
    {
        var result = await _service.Registrar(norma);
        return Ok(result);
    }
}