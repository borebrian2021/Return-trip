using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class SystemDBContextFactory : IDesignTimeDbContextFactory<SystemDBContext>
{
    public SystemDBContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<SystemDBContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ReturnTripDesignTime;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new SystemDBContext(options);
    }
}
