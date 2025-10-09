using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

[ApiController]
[Route("api/licencas")]
public class LicencasController : ControllerBase
{
    private readonly LicencasService _service;
    public LicencasController(LicencasService service) => _service = service;

    [HttpPost("validade")]
    public async Task<IActionResult> Registrar([FromBody] Licenca licenca)
    {
        var result = await _service.Registrar(licenca);
        return Ok(result);
    }

    [HttpGet("validade")]
    public IActionResult Consultar()
    {
        var result = _service.Consultar();
        return Ok(result);
    }
}