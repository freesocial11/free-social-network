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

        T First(Expression<Func<T, bool>> where);

        T FirstOrDefault(Expression<Func<T, bool>> where);

        IQueryable<T> Get(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy, bool ascending = true,
            int page = 1, int count = int.MaxValue);

        Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy, bool ascending = true,
            int page = 1, int count = int.MaxValue);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);

        Task<T> GetAsync(int id);
    }

}