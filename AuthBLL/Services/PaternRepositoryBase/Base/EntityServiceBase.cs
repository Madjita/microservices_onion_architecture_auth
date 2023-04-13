// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using MobileDrill_Common.Models;
// using MobileDrill_DAL.Entities.Base;
// using MobileDrill_DAL.Repository.Base;

// namespace MobileDrill_DAL.Service;

// /// <summary>
// ///     A base service every entity (Database Entity, Remote Entity, ...) must inherit
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// public interface IEntityServiceBase<TEntity> where TEntity : EntityBase
// {
//     /// <summary>
//     ///     Creates or Updates an Entity
//     /// </summary>
//     /// <param name="entity"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<TEntity> SaveAsync(TEntity entity, CancellationToken cancellationToken = default);

//     /// <summary>
//     ///     Deletes an Entity
//     /// </summary>
//     /// <param name="entity"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

//     /// <summary>
//     ///     Gets and Entity by its key
//     /// </summary>
//     /// <param name="id"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken = default);
// }

// /// <summary>
// ///     A common implementation for Database Entity. Call methods of this class inside implementations of
// ///     IEntityServiceBase
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// public static class DbEntityServiceBase<TBase,TEntity> 
// where TBase : DbContext
// where TEntity : EntityBase
// {
//     public static async Task<TEntity> SaveAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         TEntity entity,
//         CancellationToken cancellationToken = default
//     )
//     {
//         logger.LogInformation(Localize.Log.Method(typeof(DbEntityServiceBase<TBase,TEntity>), nameof(SaveAsync), $"{entity?.GetType().Name} {entity?.Id}"));

//         repository.Save(entity);
//         await repository.CommitAsync(cancellationToken);

//         return entity;
//     }

//     public static async Task DeleteAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         TEntity entity,
//         CancellationToken cancellationToken = default
//     )
//     {
//         logger.LogInformation(
//             Localize.Log.Method(typeof(DbEntityServiceBase<TBase,TEntity>), nameof(DeleteAsync), $"{entity?.GetType().Name} {entity?.Id}"));

//         repository.Delete(entity);
//         await repository.CommitAsync(cancellationToken);
//     }

//     public static async Task<TEntity> GetByIdAsync(ILogger logger, IRepositoryBase<TBase,TEntity> repository, int id, CancellationToken cancellationToken = default)
//     {
//         var entity = await repository.SingleOrDefaultAsync(_ => _.Id == id);

//         logger.LogInformation(Localize.Log.Method(typeof(DbEntityServiceBase<TBase,TEntity>), nameof(GetByIdAsync), $"{entity?.GetType().Name} {entity?.Id}"));

//         return entity;
//     }
// }