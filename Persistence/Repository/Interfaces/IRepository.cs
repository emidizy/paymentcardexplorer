using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task AddRecord(TEntity entity);
        IEnumerable<TEntity> GetAllRecords();
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        void UpdateRecord(TEntity entity);
    }
}
