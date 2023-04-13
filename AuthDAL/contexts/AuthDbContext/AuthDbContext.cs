using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthDAL.Entities;
using AuthDAL.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace MobileDrill.DataBase.Data;

public partial class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        // Do not migrate database on DbContext creation
        // Migrations must be done manually via dotnet-ef tool before launching any applications that might consume the context
        // Use AuthServiceV2: (add_migration.ps1, update_database.ps1, remove_migration.ps1)
        // Database.Migrate();
    }

    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<JsonWebToken> JsonWebTokens { get; set; }
    public virtual DbSet<Role> Roles { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        /*
         * SQLite doesn't natively support the following data types: DateTimeOffset, Decimal, TimeSpan, UInt64
         */
        // configurationBuilder
        //     .Properties<DateTimeOffset>()
        //     .HaveConversion<DateTimeOffsetConverter>();

        // configurationBuilder
        //     .Properties<DateTime>()
        //     .HaveConversion<DateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var seedDbSets = AuthDbContext.SeedDbSets();

        builder.Entity<Role>(_ => { _.HasData(seedDbSets.Roles); });
        builder.Entity<Account>(_ => { _.HasData(seedDbSets.Accounts); });
        builder.Entity<JsonWebToken>(_ => { _.HasIndex(__ => __.Token).IsUnique(); });
    }

    #region Helpers

    /// <summary>
    ///     Used to automatically update CreatedAt, UpdatedAt fields
    /// </summary>
    private void UpdateTimestamps()
    {
        try
        {
            var entityEntries = ChangeTracker.Entries();

            foreach (var entityEntry in entityEntries.Where(_ => _.State is EntityState.Modified or EntityState.Added))
            {
                var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;

                if (entityEntry.State == EntityState.Added)
                    ((EntityBase) entityEntry.Entity).CreatedAt = dateTimeOffsetUtcNow;
                ((EntityBase) entityEntry.Entity).UpdatedAt = dateTimeOffsetUtcNow;
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }

    #endregion

    #region Overrides

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    #endregion
}