using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


using System.Threading.Tasks;
using Data_layer.EF_entities;
using Data_layer.Data;

namespace Infrastructure_layer.Services.Repository
{
    /*
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(long id);

        IQueryable<T> Get();
        List<T> GetAll();

        Context GetContext();

        Task<long> AddAsync(T entity);
        Task<long> RemoveAsync(T entity);
        Task<long> UpdateAsync(T entity);
        Task<long> SaveAsync();


        Task<IEnumerable<T>> toListAsync();
    }*/

    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly Context _context;

        public Repository(Context context)
        {
            _context = context;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>();
        }

        public async Task<IEnumerable<T>> toListAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public Context GetContext()
        {
            return _context;
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<T> GetByIdAsync(long id)
        {
            var result = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);

            if (result == null)
            {
                //todo: need to add logger
                return null;
            }

            return result;
        }

        public async Task<long> AddAsync(T entity)
        {
            var result = await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return result.Entity.Id;
        }

        public async Task<long> RemoveAsync(T entity)
        {
            var result = _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return result.Entity.Id;
        }

        public async Task<long> UpdateAsync(T entity)
        {
            var result = _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return result.Entity.Id;
        }

        public async Task<long> SaveAsync()
        {
            await _context.SaveChangesAsync();
            return 1;
        }
    }
}

