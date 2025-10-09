public class ComplianceService
{
    private readonly AppDbContext _context;
    public ComplianceService(AppDbContext context) => _context = context;

    public async Task<Norma> Registrar(Norma norma)
    {
        _context.Normas.Add(norma);
        await _context.SaveChangesAsync();
        return norma;
    }
}