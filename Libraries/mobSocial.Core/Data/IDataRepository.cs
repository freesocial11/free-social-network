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
        /// Gets an entity by Id loading specified related entities 
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id"></param>
        /// <param name="earlyLoad"></param>
        /// <returns></returns>
        T Get<TProperty>(int id, params Expression<Func<T, TProperty>>[] earlyLoad);

        /// <summary>
        /// Gets entities matching the passed where expression
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        IQueryable<T> Get(Expression<Func<T, bool>> where);

        /// <summary>
        /// Gets entities matching the passed where expression loading specified related entities 
        /// </summary>
        /// <returns></returns>
        IQueryable<T> Get<TProperty>(Expression<Func<T, bool>> where, params Expression<Func<T, TProperty>>[] earlyLoad);

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

        /// <summary>
        /// Gets entities matching the passed where expression loading specified related entities 
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> GetAsync<TProperty>(Expression<Func<T, bool>> where, params Expression<Func<T, TProperty>>[] earlyLoad);

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);
    }
}