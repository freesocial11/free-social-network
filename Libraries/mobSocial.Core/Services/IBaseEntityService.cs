using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using mobSocial.Core.Data;

namespace mobSocial.Core.Services
{
    public interface IBaseEntityService<T> where T : BaseEntity
    {
        void Insert(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);

        void Update(T entity);

        T Get(int id);

        T Get(int id, params Expression<Func<T, object>>[] earlyLoad);

        T First(Expression<Func<T, bool>> where);

        T First(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] earlyLoad);

        T FirstOrDefault(Expression<Func<T, bool>> where);

        T FirstOrDefault(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] earlyLoad);

        int Count(Expression<Func<T, bool>> where = null);

        IQueryable<T> Get(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true,
            int page = 1, int count = int.MaxValue);

        IQueryable<T> Get(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true,
            int page = 1, int count = int.MaxValue, params Expression<Func<T, object>>[] earlyLoad);

        Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true,
            int page = 1, int count = int.MaxValue);

        Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true,
            int page = 1, int count = int.MaxValue, params Expression<Func<T, object>>[] earlyLoad);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] earlyLoad);

        Task<T> GetAsync(int id);

        Task<T> GetAsync(int id, params Expression<Func<T, object>>[] earlyLoad);

        T PreviousOrDefault(int currentEntityId, Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true);

        T NextOrDefault(int currentEntityId, Expression<Func<T, bool>> where = null, Expression<Func<T, object>> orderBy = null, bool ascending = true);
    }

}