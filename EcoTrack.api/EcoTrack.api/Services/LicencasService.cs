
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

    internal object Consultar()
    {
        throw new NotImplementedException();
    }
}
