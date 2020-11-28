using Persistence.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository.Services
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DatabaseContext _dbContext;

        public Repository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddRecord(TEntity entity)
        {
            return  _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public IEnumerable<TEntity> GetAllRecords()
        {
            return _dbContext.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public void UpdateRecord(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}
