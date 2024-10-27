using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using MySql.EntityFrameworkCore.Extensions;

namespace Infrastructure.Persistence;

public class CDbContext : DbContext
{
    private readonly ILogger<CDbContext> _logger = default!;

    public CDbContext()
    {
    }

    public CDbContext(DbContextOptions<CDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettingsMigrations.json", false, true)
            .Build();
        options.UseMySQL(configuration.GetConnectionString("Default")!);
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Ident> Idents => Set<Ident>();

    public async Task<bool> SaveAllChangesAsync()
    {
        _logger?.LogDebug("\n{output}", ChangeTracker.DebugView.LongView);

        var result = await SaveChangesAsync();

        _logger?.LogDebug("SaveChanges {result}", result);
        _logger?.LogDebug("\n{output}", ChangeTracker.DebugView.LongView);
        return result > 0;
    }
}