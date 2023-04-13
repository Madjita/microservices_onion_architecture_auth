using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MobileDrill_DAL.Repository.Base;
using MobileDrill.DataBase.Data;
using AuthDAL.Entities.Base;

namespace AuthBLL.Services.Repository
{
    public interface IRepository<T>: IRepositorySync<T>, IRepositoryAsync<T>, IRepositoryBase<AuthDbContext,T> where T : EntityBase
    {
        AuthDbContext GetContext();
    }

    public interface IRepositorySync<T> where T : EntityBase
    {
        IQueryable<T> Get();
        List<T> GetAll();
    }

    public interface IRepositoryAsync<T> where T : EntityBase
    {
        Task<T> GetByIdAsync(long id);
        Task<long> AddAsync(T entity);
        Task<long> RemoveAsync(T entity);
        Task<long> UpdateAsync(T entity);
        Task<long> SaveAsync();
        Task<IEnumerable<T>> toListAsync();
    }
}

