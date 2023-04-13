using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MobileDrill.DataBase.Data;

public interface IAppDbContextTransactionAction<TBase>
where TBase : DbContext 
{
    DbContext GetAppDbContext();
    Task<bool> CanConnectAsync();
    Task<bool> EnsureCreated();
    void BeginTransaction(bool shouldThrow = false);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false);
    void CommitTransaction(bool shouldThrow = false);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false);
    void RollbackTransaction(bool shouldThrow = false);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false);
}

public interface IAppDbContextEntityAction<TBase>
where TBase : DbContext 
{
    void Commit();
    Task CommitAsync(CancellationToken cancellationToken = default);
}

/// <summary>
///     Use of this service through DI requires you to have ServiceLifetime.Scoped DbContext.
///     This way you are able to achieve single transaction control over multiple repository-pattern DbContexts.
///     All repositories implement interfaces to control their own DbContext transaction.
///     If the passed DbContext in repository is created via ServiceLifetime.Transient or via DbContext Factory
/// </summary>
public class AppDbContextAction<TBase> : IAppDbContextEntityAction<TBase>, IAppDbContextTransactionAction<TBase>
    where TBase : DbContext 
{
    private readonly DbContext _appDbContext;
    private readonly ILogger<TBase> _logger;

    public AppDbContextAction(TBase appDbContext)
    {
        _appDbContext = (DbContext)appDbContext;
    }

    private bool TransactionInProgress { get; set; }

    public void Commit()
    {
        _appDbContext.SaveChanges();
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _appDbContext.SaveChangesAsync(cancellationToken);
    }

    public DbContext GetAppDbContext()
    {
        return _appDbContext;
    }

    public void BeginTransaction(bool shouldThrow = false)
    {
        if (TransactionInProgress)
        {
            if (shouldThrow)
                throw new System.Exception();

        }
        else
        {
            _appDbContext.Database.BeginTransaction();
            TransactionInProgress = true;
        }

    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false)
    {

        if (TransactionInProgress)
        {
            if (shouldThrow)
                throw new System.Exception();

        }
        else
        {
            await _appDbContext.Database.BeginTransactionAsync(cancellationToken);
            TransactionInProgress = true;
        }
    }

    public void CommitTransaction(bool shouldThrow = false)
    {

        if (!TransactionInProgress)
        {
            if (shouldThrow)
                throw new System.Exception();

        }
        else
        {
            _appDbContext.Database.CommitTransaction();
            TransactionInProgress = false;
        }

    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false)
    {

        if (!TransactionInProgress)
        {
            if (shouldThrow)
                throw new System.Exception();
        }
        else
        {
            await _appDbContext.Database.CommitTransactionAsync(cancellationToken);
            TransactionInProgress = false;
        }
    }

    public void RollbackTransaction(bool shouldThrow = false)
    {

        if (TransactionInProgress)
        {
            _appDbContext.Database.RollbackTransaction();
        }
        else
        {
            if (shouldThrow)
               throw new System.Exception();
        }

    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default, bool shouldThrow = false)
    {

        if (TransactionInProgress)
        {
            await _appDbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
        else
        {
            if (shouldThrow)
              throw new System.Exception();

        }
    }

    public async Task<bool> CanConnectAsync()
    {
        return await _appDbContext.Database.CanConnectAsync();
    }

    public async Task<bool> EnsureCreated()
    {
        return await _appDbContext.Database.EnsureCreatedAsync();
    }
}