public class EmissoesService
{
    private readonly AppDbContext _context;
    public EmissoesService(AppDbContext context) => _context = context;

    public async Task<Emissao> Monitorar(Emissao emissao)
    {
        _context.Emissoes.Add(emissao);
        await _context.SaveChangesAsync();
        return emissao;
    }
}