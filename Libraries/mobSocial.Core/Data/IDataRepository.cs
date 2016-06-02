using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace mobSocial.Core.Data
{
    public interface IDataRepository<T> where T: BaseEntity
    {
        /// <summary>
        /// Gets an entity by Id
        /// </summary>
        /// <param name="id">The Id of entity</param>
        /// <returns>Entity with Id value</returns>
        T Get(int id);

        /// <summary>
        /// Gets entities matching the passed where expression
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IQueryable<T> Get(Expression<Func<T, bool>> where);

        /// <summary>
        /// Gets total number of entities matching where expression
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        int Count(Expression<Func<T, bool>> where);

        /// <summary>
        /// Gets entities matching the passed where expression asynchronously
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> where);

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);
    }
}