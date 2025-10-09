using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

// Ajuste estes namespaces se necessário, dependendo de como estão no seu projeto:
using EcoTrack.api;

namespace EcoTrack.Tests.Database;

public class LicencasDbTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public void Model_deve_conter_entidade_Licenca()
    {
        using var ctx = CreateInMemoryContext();
        var hasLicenca = ctx.Model.GetEntityTypes().Any(t => t.ClrType.Name.Equals("Licenca", StringComparison.OrdinalIgnoreCase));
        hasLicenca.Should().BeTrue("o AppDbContext precisa mapear a entidade 'Licenca'");
    }
}
