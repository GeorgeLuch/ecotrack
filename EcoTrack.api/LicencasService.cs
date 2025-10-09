public class LicencasService
{
    private readonly AppDbContext _context;
    public LicencasService(AppDbContext context) => _context = context;

    public async Task<Licenca> Registrar(Licenca licenca)
    {
        _context.Licencas.Add(licenca);
        await _context.SaveChangesAsync();
        return licenca;
    }

    public List<Licenca> Consultar()
    {
        var hoje = DateTime.Today;
        return _context.Licencas.ToList().Select(l =>
        {
            var diasRestantes = (l.Validade - hoje).TotalDays;
            return l;
        }).ToList();
    }
}