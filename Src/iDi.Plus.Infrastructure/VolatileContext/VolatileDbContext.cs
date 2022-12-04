using System.Reflection;
using iDi.Plus.Domain.Entities;
using iDi.Plus.Infrastructure.VolatileContext.Configurations;
using Microsoft.EntityFrameworkCore;

namespace iDi.Plus.Infrastructure.VolatileContext;

public class VolatileDbContext : DbContext
{
    private readonly string _dbPath;

    public DbSet<HotPoolRecord> HotPool { get; set; }
    public DbSet<ConsentRequest> ConsentRequests { get; set; }
    public DbSet<ConsentTx> Consents { get; set; }
    
    public VolatileDbContext()
    {
        var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        _dbPath = Path.Join(path, "IdPlus_Volatile.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ConsentTxConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={_dbPath}");

    public void ApplyMigrations(Action<VolatileDbContext> seedMethod = null)
    {
        if (Database.GetPendingMigrations().Any())
            Database.Migrate();

        if (seedMethod != null)
            seedMethod(this);
    }
}