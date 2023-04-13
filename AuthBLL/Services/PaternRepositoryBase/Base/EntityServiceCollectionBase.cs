// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using MobileDrill_Common.Models;
// using MobileDrill_DAL.Entities.Base;
// using MobileDrill_DAL.Extensions;
// using MobileDrill_DAL.Repository.Base;

// namespace MobileDrill_DAL.Service;

// /// <summary>
// ///     A base service some entities (Database Entity, Remote Entity, ...) might implement
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// public interface IEntityServiceCollectionBase<TEntity> where TEntity : EntityBase
// {
//     /// <summary>
//     ///     Creates or Saves Entities
//     /// </summary>
//     /// <param name="entities"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<IReadOnlyCollection<TEntity>> SaveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

//     /// <summary>
//     ///     Deletes Entities
//     /// </summary>
//     /// <param name="entities"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

//     /// <summary>
//     ///     Gets a collection of Entities
//     /// </summary>
//     /// <param name="logger"></param>
//     /// <param name="pageModel">Pagination model</param>
//     /// <param name="queryTransformationFunction">Transforms query with custom logic</param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<(int total, IReadOnlyCollection<TEntity> entities)> GetCollection(
//         ILogger logger,
//         PageModel pageModel,
//         Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformationFunction,
//         CancellationToken cancellationToken = default
//     );

//     /// <summary>
//     ///     Gets a collection of Entities
//     /// </summary>
//     /// <param name="logger"></param>
//     /// <param name="pageModel">Pagination model</param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<(int total, IReadOnlyCollection<TEntity> entities)> GetCollection(
//         ILogger logger,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     );
// }

// /// <summary>
// ///     A common implementation for Database Entity. Call methods of this class inside implementations of
// ///     IEntityServiceCollectionBase
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// public static class DbEntityServiceCollectionBase<TBase,TEntity> 
// where TBase : DbContext
// where TEntity : EntityBase
// {
//     public static async Task<IReadOnlyCollection<TEntity>> SaveAsync(
//         ILogger logger,
//         IRepositoryBase<TBase, TEntity> repository,
//         IEnumerable<TEntity> entities,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var entitiesEnumerated = entities as TEntity[] ?? entities.ToArray();
//         logger.LogInformation(Localize.Log.Method(typeof(DbEntityServiceBase<TBase, TEntity>), nameof(SaveAsync), $"{entities?.GetType().Name} {entitiesEnumerated.Length}"));

//         repository.Save(entitiesEnumerated);
//         await repository.CommitAsync(cancellationToken);

//         return entitiesEnumerated;
//     }

//     public static async Task DeleteAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         IEnumerable<TEntity> entities,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var entitiesEnumerated = entities as TEntity[] ?? entities.ToArray();
//         logger.LogInformation(
//             Localize.Log.Method(typeof(DbEntityServiceBase<TBase,TEntity>), nameof(DeleteAsync), $"{entities?.GetType().Name} {entitiesEnumerated.Length}"));

//         repository.Delete(entitiesEnumerated);
//         await repository.CommitAsync(cancellationToken);
//     }

//     public static async Task<(int total, IReadOnlyCollection<TEntity> entities)> GetCollection(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         PageModel pageModel,
//         Func<IQueryable<TEntity>, IQueryable<TEntity>> queryTransformationFunction,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var result = repository.QueryMany(queryTransformationFunction);

//         var total = result.Count();

//         result = result.GetPage(pageModel);

//         logger.Log(LogLevel.Information, Localize.Log.Method(typeof(DbEntityServiceCollectionBase<TBase,TEntity>), nameof(GetCollection), $"{result?.GetType().Name} {result?.Count()}"));

//         return (total, await result?.ToArrayAsync(cancellationToken)!);
//     }

//     public static async Task<(int total, IReadOnlyCollection<TEntity> entities)> GetCollection(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var result = repository.QueryAll();

//         var total = result.Count();

//         result = result.GetPage(pageModel);

//         logger.Log(LogLevel.Information, Localize.Log.Method(typeof(DbEntityServiceCollectionBase<TBase,TEntity>), nameof(GetCollection), $"{result?.GetType().Name} {result?.Count()}"));

//         return (total, await result?.ToArrayAsync(cancellationToken)!);
//     }
// }