using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _context;
    public RelatoriosController(AppDbContext context) => _context = context;

    [HttpGet("resumo")]
    public IActionResult Resumo([FromQuery] DateTime inicio, [FromQuery] DateTime fim, [FromQuery] string tipo)
    {
        var resumo = new RelatorioResumo
        {
            Normas = tipo == "compliance" || tipo == "todos" ? _context.Normas.Where(n => n.Data >= inicio && n.Data <= fim).ToList() : new(),
            Emissoes = tipo == "emissao" || tipo == "todos" ? _context.Emissoes.Where(e => e.Data >= inicio && e.Data <= fim).ToList() : new(),
            Licencas = tipo == "licencas" || tipo == "todos" ? _context.Licencas.Where(l => l.Validade >= inicio && l.Validade <= fim).ToList() : new()
        };
        return Ok(resumo);
    }
}