using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/emissoes")]
public class EmissoesController : ControllerBase
{
    private readonly EmissoesService _service;
    public EmissoesController(EmissoesService service) => _service = service;

    [HttpPost("monitorar")]
    public async Task<IActionResult> Monitorar([FromBody] Emissao emissao)
    {
        var result = await _service.Monitorar(emissao);
        return Ok(result);
    }
}