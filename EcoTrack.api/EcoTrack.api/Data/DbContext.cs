using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Norma> Normas { get; set; }
    public DbSet<Emissao> Emissoes { get; set; }
    public DbSet<Licenca> Licencas { get; set; }
}