// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using MobileDrill_Common.Models;
// using MobileDrill_DAL.Entities.Base;
// using MobileDrill_DAL.Extensions;
// using MobileDrill_DAL.Repository.Base;

// namespace MobileDrill_DAL.Service;

// /// <summary>
// ///     A base service every mapping entity (Database Entity, Remote Entity, ...) must inherit. This is commonly used in
// ///     Many-to-Many Database schemes
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// public interface IEntityToEntityMappingServiceBase<TEntity> : IEntityServiceBase<TEntity> where TEntity : EntityBase
// {
//     /// <summary>
//     ///     Gets an Entity by composite key
//     /// </summary>
//     /// <param name="entityLeftId"></param>
//     /// <param name="entityRightId"></param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<TEntity> GetByEntityLeftIdEntityRightIdAsync(
//         int entityLeftId,
//         int entityRightId,
//         CancellationToken cancellationToken = default
//     );

//     /// <summary>
//     ///     Gets an Entity by a part of composite key
//     /// </summary>
//     /// <param name="entityLeftId"></param>
//     /// <param name="pageModel">Pagination model</param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<(int total, IReadOnlyCollection<TEntity> entities)> GetByEntityLeftIdAsync(
//         int entityLeftId,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     );

//     /// <summary>
//     ///     Gets an Entity by a part of composite key
//     /// </summary>
//     /// <param name="entityRightId"></param>
//     /// <param name="pageModel">Pagination model</param>
//     /// <param name="cancellationToken"></param>
//     /// <returns></returns>
//     Task<(int total, IReadOnlyCollection<TEntity> entities)> GetByEntityRightIdAsync(
//         int entityRightId,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     );
// }

// /// <summary>
// ///     A common implementation for Database Entity. Call methods of this class inside implementations of
// ///     IEntityToEntityMappingServiceBase
// /// </summary>
// /// <typeparam name="TEntity"></typeparam>
// /// <typeparam name="TEntityLeft"></typeparam>
// /// <typeparam name="TEntityRight"></typeparam>
// public static class DbEntityToEntityMappingServiceBase<TBase,TEntity, TEntityLeft, TEntityRight>
//     where TBase : DbContext 
//     where TEntity : EntityToEntityMappingBase<TEntityLeft, TEntityRight>
//     where TEntityLeft : EntityBase
//     where TEntityRight : EntityBase
// {
//     public static async Task<TEntity> GetByEntityLeftIdEntityRightIdAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         int entityLeftId,
//         int entityRightId,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var entity = await repository.SingleOrDefaultAsync(_ =>
//             _.EntityLeftId == entityLeftId && _.EntityRightId == entityRightId);

//         logger.LogInformation(
//             Localize.Log.Method(typeof(DbEntityToEntityMappingServiceBase<TBase,TEntity, TEntityLeft, TEntityRight>), nameof(GetByEntityLeftIdEntityRightIdAsync),
//                 $"{entity?.GetType().Name} {entity?.Id}"));

//         return entity;
//     }

//     public static async Task<(int total, IReadOnlyCollection<TEntity> entities)> GetByEntityLeftIdAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         int entityLeftId,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var query = repository
//             .QueryMany(_ => _.EntityLeftId == entityLeftId)
//             .OrderBy(_ => _.CreatedAt);

//         var total = query.Count();

//         var result = query.GetPage(pageModel);

//         logger.LogInformation(
//             Localize.Log.Method(typeof(DbEntityToEntityMappingServiceBase<TBase,TEntity, TEntityLeft, TEntityRight>), nameof(GetByEntityLeftIdAsync),
//                 $"{result.GetType().Name} {result.Count()}"));

//         return (total, await result.ToArrayAsync(cancellationToken)!);
//     }

//     public static async Task<(int total, IReadOnlyCollection<TEntity> entities)> GetByEntityRightIdAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         int entityRightId,
//         PageModel pageModel,
//         CancellationToken cancellationToken = default
//     )
//     {
//         var query = repository
//             .QueryMany(_ => _.EntityRightId == entityRightId)
//             .OrderBy(_ => _.CreatedAt);

//         var total = query.Count();

//         var result = query.GetPage(pageModel);

//         logger.LogInformation(
//             Localize.Log.Method(typeof(DbEntityToEntityMappingServiceBase<TBase,TEntity, TEntityLeft, TEntityRight>), nameof(GetByEntityRightIdAsync),
//                 $"{result.GetType().Name} {result.Count()}"));

//         return (total, await result.ToArrayAsync(cancellationToken)!);
//     }
// }