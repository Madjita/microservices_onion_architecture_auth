// using Microsoft.Extensions.Logging;
// using MobileDrill_DAL.Entities;
// using MobileDrill_DAL.Repository.Base;
// using MobileDrill.DataBase.Data;
// using AppDbContext = MobileDrill.DataBase.Data.AppDbContext;
// using Rit.Models.Database;

// namespace MobileDrill_DAL.Repository;

// public interface IJsonWebTokenRepository : IRepositoryBase<AppDbContext,JsonWebToken>
// {
// }

// public class JsonWebTokenRepository : RepositoryBase<AppDbContext,JsonWebToken>, IJsonWebTokenRepository
// {
//     public JsonWebTokenRepository(AppDbContext appDbContext, ILogger<AppDbContext> loggerAppDbContextAction) : base((AppDbContext)appDbContext, loggerAppDbContextAction)
//     {
//     }
// }