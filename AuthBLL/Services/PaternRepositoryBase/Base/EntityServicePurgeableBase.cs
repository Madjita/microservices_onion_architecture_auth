// using System.Linq.Expressions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using MobileDrill_Common.Models;
// using MobileDrill_DAL.Entities.Base;
// using MobileDrill_DAL.Repository.Base;

// namespace MobileDrill_DAL.Service;

// public interface IEntityServicePurgeableBase<TEntity> where TEntity : EntityBase
// {
//     Task PurgeAsync(CancellationToken cancellationToken = default);
// }

// public static class DbEntityServicePurgeableBase<TBase,TEntity> 
// where TBase : DbContext
// where TEntity : EntityBase
// {
//     public static async Task PurgeAsync(
//         ILogger logger,
//         IRepositoryBase<TBase,TEntity> repository,
//         Expression<Func<TEntity, bool>> predicate,
//         CancellationToken cancellationToken = default
//     )
//     {
//         logger.LogInformation(Localize.Log.Method(typeof(DbEntityServicePurgeableBase<TBase,TEntity>), nameof(PurgeAsync), null));

//         await repository
//             .QueryMany(predicate)
//             .ExecuteDeleteAsync(cancellationToken);

//         await repository.CommitAsync(cancellationToken);
//     }
// }