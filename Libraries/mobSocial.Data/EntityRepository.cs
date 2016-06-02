using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using mobSocial.Core.Data;
using mobSocial.Data.Database;

namespace mobSocial.Data
{
    public class EntityRepository<T> : IDataRepository<T> where T : BaseEntity
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly IDbSet<T> _entityDbSet;

        public EntityRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _entityDbSet = _databaseContext.Set<T>();
        }

        public T Get(int id)
        {
            return _entityDbSet.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> @where)
        {
            return _entityDbSet.Where(where);
        }

        public int Count(Expression<Func<T, bool>> @where)
        {
            return _entityDbSet.Count(where);
        }

        public async Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => Get(@where));
        }

        public void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException();

            try
            {
                _entityDbSet.Add(entity);
                _databaseContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

            }
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException();

            try
            {
                _databaseContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

            }
        }

        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException();

            try
            {
                //if it's soft deletable, we should just set deleted to true, instead of deleting 
                var deletable = entity as ISoftDeletable;
                if (deletable != null)
                {
                    deletable.Deleted = true;
                    Update(entity);
                    return;
                }
                _entityDbSet.Remove(entity);

                _databaseContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

            }
        }

        public void Delete(Expression<Func<T, bool>> @where)
        {

            try
            {
                var entities = _entityDbSet.Where(where);
                foreach (var entity in entities)
                {
                    var deletable = entity as ISoftDeletable;
                    if (deletable != null)
                    {
                        deletable.Deleted = true;
                        Update(entity);
                        continue;
                    }
                    _entityDbSet.Remove(entity);

                }

                _databaseContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

            }
        }
    }
}