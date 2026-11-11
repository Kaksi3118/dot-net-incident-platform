using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpsRelay.Infrastructure.Persistence;

public class OpsRelayDbContextFactory : IDesignTimeDbContextFactory<OpsRelayDbContext>
{
    private const string DesignTimeConnectionString =
        "Server=localhost,1433;Database=OpsRelayDb;User Id=sa;Password=OpsRelay_Dev!2024;TrustServerCertificate=True;";

    public OpsRelayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OpsRelayDbContext>();
        optionsBuilder.UseSqlServer(DesignTimeConnectionString);

        return new OpsRelayDbContext(optionsBuilder.Options);
    }
}
