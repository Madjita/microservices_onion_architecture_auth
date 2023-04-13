using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthDAL.Entities.Base;
using MobileDrill.DataBase.Data;
using MobileDrill_DAL.Repository.Base;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace AuthBLL.Services.Repository
{
    public class Repository<T> : RepositoryBase<AuthDbContext,T>, IRepository<T> where T : EntityBase
    {
        private readonly AuthDbContext _context;

        public Repository(AuthDbContext context,ILogger<AuthDbContext> loggerDbContextAction):base(context,loggerDbContextAction)
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

        public AuthDbContext GetContext()
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

