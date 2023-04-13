using System;
using AuthDAL.Data;
using AuthDAL.EF_entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthBLL.Services.Repository
{
    public interface IRepository<T>: IRepositorySync<T>, IRepositoryAsync<T> where T : BaseEntity
    {
        Context GetContext();
    }

    public interface IRepositorySync<T> where T : BaseEntity
    {
        IQueryable<T> Get();
        List<T> GetAll();
    }

    public interface IRepositoryAsync<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(long id);
        Task<long> AddAsync(T entity);
        Task<long> RemoveAsync(T entity);
        Task<long> UpdateAsync(T entity);
        Task<long> SaveAsync();
        Task<IEnumerable<T>> toListAsync();
    }
}

