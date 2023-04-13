using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AuthDAL.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MobileDrill.DataBase.Data;


namespace MobileDrill_DAL.Repository.Base;

public interface IRepositoryBase
{
    /// <summary>
    ///     Gets type of a generic used in implementation
    /// </summary>
    /// <returns></returns>
    Type GetEntityType();

    /// <summary>
    ///     Gets a table name from Database. Can be used in raw sql code
    /// </summary>
    /// <returns></returns>
    string GetTableName();
}

public interface IRepositoryBase<TBase,TEntity> : IRepositoryBase, IDbContextEntityAction<TBase>
    where TBase : DbContext
    where TEntity : EntityBase
{
    /// <summary>
    ///     Creates or Updates an Entity
    /// </summary>
    /// <param name="entity"></param>
    void Save(TEntity entity);

    /// <summary>
    ///     Creates or Updates an Entity, afterwards commits changes
    /// </summary>
    /// <param name="entity"></param>
    void SaveAndCommit(TEntity entity);

    /// <summary>
    ///     Creates or Updates an Entity, afterwards commits changes
    /// </summary>
    /// <param name="entity"></param>
    Task SaveAndCommitAsync(TEntity entity);

    /// <summary>
    ///     Creates or Updates Entities
    /// </summary>
    /// <param name="entities"></param>
    void Save(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Creates or Updates Entities, afterwards commits changes
    /// </summary>
    /// <param name="entities"></param>
    void SaveAndCommit(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Creates or Updates Entities, afterwards commits changes
    /// </summary>
    /// <param name="entities"></param>
    Task SaveAndCommitAsync(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Deletes an Entity
    /// </summary>
    /// <param name="entity"></param>
    void Delete(TEntity entity);

    /// <summary>
    ///     Deletes an Entity, afterwards commits changes
    /// </summary>
    /// <param name="entity"></param>
    void DeleteAndCommit(TEntity entity);

    /// <summary>
    ///     Deletes an Entity, afterwards commits changes
    /// </summary>
    /// <param name="entity"></param>
    Task DeleteAndCommitAsync(TEntity entity);

    /// <summary>
    ///     Deletes Entities
    /// </summary>
    /// <param name="entities"></param>
    void Delete(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Deletes Entities, afterwards commits changes
    /// </summary>
    /// <param name="entities"></param>
    void DeleteAndCommit(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Deletes Entities, afterwards commits changes
    /// </summary>
    /// <param name="entities"></param>
    Task DeleteAndCommitAsync(IEnumerable<TEntity> entities);

    /// <summary>
    ///     Gets an Entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<TEntity> GetByIdAsync(int id, bool asNoTracking = false);

    /// <summary>
    ///     Gets an Entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<TEntity> GetByIdOrDefaultAsync(int id, bool asNoTracking = false);

    /// <summary>
    ///     Gets an Entity by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

    /// <summary>
    ///     Gets and Entity by predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="asNoTracking"></param>
    /// <returns></returns>
    Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

    IQueryable<TResult> QueryMany<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        bool asNoTracking = false
    );

    IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);
    IQueryable<TEntity> QueryMany(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformationFunction, bool asNoTracking = false);
    IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector, bool asNoTracking = false);
    IQueryable<TEntity> QueryAll(bool asNoTracking = false);
    IQueryable<TEntity> FromSql(string sql, bool asNoTracking = false, params object[] parameters);
    Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task LoadCollectionAsync(TEntity entity, Expression<Func<TEntity, IEnumerable<object>>> collectionExpression);
    Task LoadReferenceAsync(TEntity entity, Expression<Func<TEntity, object?>> collectionExpression);
}

public class RepositoryBase<TBase,TEntity> : IRepositoryBase<TBase,TEntity>, IDbContextEntityAction<TBase>
    where TBase : DbContext
    where TEntity : EntityBase
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly AppDbContextAction<TBase> _dbContextAction;

    public RepositoryBase(TBase context, ILogger<TBase> loggerDbContextAction)
    {
        _context = (DbContext)context;
        _dbSet = _context.Set<TEntity>();
        _dbContextAction = new AppDbContextAction<TBase>(context, loggerDbContextAction);
    }


    public void Save(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void SaveAndCommit(TEntity entity)
    {
        Save(entity);
        _dbContextAction.Commit();
    }

    public async Task SaveAndCommitAsync(TEntity entity)
    {
        Save(entity);
        await _dbContextAction.CommitAsync();
    }

    public void Save(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void SaveAndCommit(IEnumerable<TEntity> entities)
    {
        Save(entities);
        _dbContextAction.Commit();
    }

    public async Task SaveAndCommitAsync(IEnumerable<TEntity> entities)
    {
        Save(entities);
        await _dbContextAction.CommitAsync();
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void DeleteAndCommit(TEntity entity)
    {
        Delete(entity);
        _dbContextAction.Commit();
    }

    public async Task DeleteAndCommitAsync(TEntity entity)
    {
        Delete(entity);
        await _dbContextAction.CommitAsync();
    }

    public void Delete(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void DeleteAndCommit(IEnumerable<TEntity> entities)
    {
        Delete(entities);
        _dbContextAction.Commit();
    }

    public async Task DeleteAndCommitAsync(IEnumerable<TEntity> entities)
    {
        Delete(entities);
        await _dbContextAction.CommitAsync();
    }

    public async Task<TEntity> GetByIdAsync(int id, bool asNoTracking = false)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().SingleAsync(_ => _.Id.Equals(id));

        return await _dbSet.SingleAsync(_ => _.Id.Equals(id));
    }

    public async Task<TEntity> GetByIdOrDefaultAsync(int id, bool asNoTracking = false)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().SingleOrDefaultAsync(_ => _.Id.Equals(id));

        return await _dbSet.SingleOrDefaultAsync(_ => _.Id.Equals(id));
    }

    public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().SingleAsync(predicate);

        return await _dbSet.SingleAsync(predicate);
    }

    public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().SingleOrDefaultAsync(predicate);

        return await _dbSet.SingleOrDefaultAsync(predicate);
    }

    public IQueryable<TResult> QueryMany<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        bool asNoTracking = false
    )
    {
        if (asNoTracking)
            return _dbSet.AsNoTracking().Where(predicate).Select(selector);

        return _dbSet.Where(predicate).Select(selector);
    }

    public IQueryable<TEntity> QueryMany(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
    {
        if (asNoTracking)
            return _dbSet.AsNoTracking().Where(predicate);

        return _dbSet.Where(predicate);
    }

    public IQueryable<TResult> QueryAll<TResult>(Expression<Func<TEntity, TResult>> selector, bool asNoTracking = false)
    {
        if (asNoTracking)
            return _dbSet.AsNoTracking().Select(selector);

        return _dbSet.Select(selector);
    }

    public IQueryable<TEntity> QueryAll(bool asNoTracking = false)
    {
        if (asNoTracking)
            return _dbSet.AsNoTracking();

        return _dbSet;
    }

    public IQueryable<TEntity> FromSql(string sql, bool asNoTracking = false, params object[] parameters)
    {
        if (asNoTracking)
            return _dbSet.FromSqlRaw(sql, parameters).AsNoTracking();

        return _dbSet.FromSqlRaw(sql, parameters);
    }

    public Type GetEntityType()
    {
        return typeof(TEntity);
    }

    public string GetTableName()
    {
        var model = _context.Model;
        var entityTypes = model.GetEntityTypes();
        var entityType = entityTypes.First(t => t.ClrType == typeof(TEntity));
        var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");
        return tableNameAnnotation.Value?.ToString();
    }

    public IQueryable<TEntity> QueryMany(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformationFunction, bool asNoTracking = false)
    {
        return queryTransformationFunction(QueryAll(asNoTracking));
    }

    public void Commit()
    {
        _dbContextAction.Commit();
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _dbContextAction.CommitAsync(cancellationToken);
    }

    public async Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate,bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if(asNoTracking)
            return await _dbSet.AsQueryable().Where(predicate).ToListAsync();

        return await _dbSet.AsQueryable().AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task LoadCollectionAsync(TEntity entity, Expression<Func<TEntity, IEnumerable<object>>> collectionExpression)
    {
        var entityEntry = _context.Entry(entity);
        if(entityEntry.State is not EntityState.Modified)
        {
            entityEntry.State = EntityState.Modified;
        }
        await entityEntry.Collection(collectionExpression).LoadAsync();
    }

    public async Task LoadReferenceAsync(TEntity entity, Expression<Func<TEntity, object?>> collectionExpression)
    {
        var entityEntry = _context.Entry(entity);
        var reference = entityEntry.Reference(collectionExpression);

        if (reference.CurrentValue == null)
        {
            await reference.LoadAsync();
        }
    }
}