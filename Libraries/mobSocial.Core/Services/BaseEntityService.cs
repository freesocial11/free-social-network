using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using mobSocial.Core.Data;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Core.Services.Events;

namespace mobSocial.Core.Services
{
    public abstract class BaseEntityService<T> : IBaseEntityService<T> where T : BaseEntity
    {
        private readonly IDataRepository<T> _dataRepository;
        private readonly IEventPublisherService _eventPublisherService;

        protected BaseEntityService(IDataRepository<T> dataRepository)
        {
            _dataRepository = dataRepository;
            //resolve publisher manually
            _eventPublisherService = mobSocialEngine.ActiveEngine.Resolve<IEventPublisherService>();
        }

        public virtual void Insert(T entity)
        {
            _dataRepository.Insert(entity);
            //publish the event so they can be handled
            _eventPublisherService.Publish(entity, EventType.Insert);
        }

        public virtual void Delete(T entity)
        {
            var deletable = entity as ISoftDeletable;
            if (deletable != null)
            {
                var entityAsSoftDeletable = deletable;
                entityAsSoftDeletable.Deleted = true;
                Update(entity);
            }
            else
            {
                //publish the event so they can be handled
                _eventPublisherService.Publish(entity, EventType.Delete);

                _dataRepository.Delete(entity);
                
            }
        }

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            if (typeof (T) is ISoftDeletable)
            {
                //it's soft deletable so it's better to retrive them all and mark them deleted
                var allEntities = Get(where, null);
                foreach (var entity in allEntities)
                {
                    var deletable = entity as ISoftDeletable;
                    if (deletable != null)
                    {
                        var entityAsSoftDeletable = deletable;
                        entityAsSoftDeletable.Deleted = true;
                        Update(entity);
                    }
                }
            }
            else
            {
                _dataRepository.Delete(where);
                
            }
        }

        public virtual void Update(T entity)
        {
            _dataRepository.Update(entity);

            //publish the event so they can be handled
            _eventPublisherService.Publish(entity, EventType.Update);
        }

        public virtual T Get(int id)
        {
            return _dataRepository.Get(x => x.Id == id).FirstOrDefault();
        }

        public virtual T First(Expression<Func<T, bool>> @where)
        {
            return _dataRepository.Get(where).First();
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> @where)
        {
            return _dataRepository.Get(where).FirstOrDefault();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> where, Expression<Func<T, object>> orderBy, bool ascending = true, int page = 1, int count = int.MaxValue)
        {
            if (where == null)
                where = (x => true);

            var resultSet = _dataRepository.Get(@where);

            if (orderBy != null)
                //order
                resultSet = ascending ? resultSet.OrderBy(orderBy) : resultSet.OrderByDescending(orderBy);
            else
                resultSet = resultSet.OrderBy(x => x.Id);

            //pagination
            resultSet = resultSet.Skip((page - 1)*count).Take(count);
            return resultSet;
            
        }

        public virtual async Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> @where, Expression<Func<T, object>> orderBy, bool @ascending = true, int page = 1, int count = Int32.MaxValue)
        {
            return await Task.Run(() => Get(@where, orderBy, ascending, page, count));
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => FirstOrDefault(where));
        }

        public virtual async Task<T> GetAsync(int id)
        {
            return await Task.Run(() => Get(id));
        }

        protected IDataRepository<T> Repository => _dataRepository;
    }
}