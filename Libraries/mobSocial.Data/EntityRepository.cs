using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DryIoc;
using mobSocial.Core;
using mobSocial.Core.Data;
using mobSocial.Core.Helpers;
using mobSocial.Core.Infrastructure.AppEngine;
using mobSocial.Data.Constants;
using mobSocial.Data.Database;
using mobSocial.Data.Entity.OAuth;

namespace mobSocial.Data
{
    public class EntityRepository<T> : IDataRepository<T> where T : BaseEntity
    {
        private IDatabaseContext _databaseContext;
        private IDbSet<T> _entityDbSet;

        private readonly string _contextServiceKey = string.Empty;

        public EntityRepository() { }

        public EntityRepository(string contextServiceKey)
        {
            _contextServiceKey = contextServiceKey;
        }

        private void _SetupContexts()
        {
            var scopedContainer = mobSocialEngine.ActiveEngine.IocContainer;
            if (scopedContainer.GetCurrentScope() == null)
                //this might be an mvc request
                scopedContainer = mobSocialEngine.ActiveEngine.MvcContainer;
            _databaseContext = _contextServiceKey == string.Empty ? scopedContainer.Resolve<IDatabaseContext>() : scopedContainer.Resolve<IDatabaseContext>(serviceKey: _contextServiceKey);

            _entityDbSet = _databaseContext.Set<T>();
        }

        public T Get(int id)
        {
            _SetupContexts();
            return _entityDbSet.FirstOrDefault(x => x.Id == id);
        }

        public T Get<TProperty>(int id, params Expression<Func<T, TProperty>>[] earlyLoad)
        {
           _SetupContexts();
            var dbSet = _entityDbSet.AsQueryable();
            dbSet = dbSet.Where(x => x.Id == id);
            dbSet = earlyLoad.Aggregate(dbSet, (current, el) => current.Include(el));

            return dbSet.FirstOrDefault();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> @where)
        {
            _SetupContexts();
            where = AppendAdditionalConditions(where);
            return _entityDbSet.Where(where);
        }

        public IQueryable<T> Get<TProperty>(Expression<Func<T, bool>> @where, params Expression<Func<T, TProperty>>[] earlyLoad)
        {
            _SetupContexts();
            where = AppendAdditionalConditions(where);
            var dbSet = _entityDbSet.AsQueryable();
            dbSet = earlyLoad.Aggregate(dbSet, (current, el) => current.Include(el));
            return dbSet.Where(where);
        }

        public int Count(Expression<Func<T, bool>> @where)
        {
            _SetupContexts();
            where = AppendAdditionalConditions(where);
            return _entityDbSet.Count(where);
        }

        public async Task<IQueryable<T>> GetAsync(Expression<Func<T, bool>> @where)
        {
            return await Task.Run(() => Get(@where));
        }

        public async Task<IQueryable<T>> GetAsync<TProperty>(Expression<Func<T, bool>> @where, params Expression<Func<T, TProperty>>[] earlyLoad)
        {
            return await Task.Run(() => Get(@where, earlyLoad));
        }

        public void Insert(T entity, bool reloadNavigationProperties = false)
        {
            _SetupContexts();
            if (entity == null)
                throw new ArgumentNullException();

            try
            {
                var applicationEntity = entity as IPerApplicationEntity;
                if (applicationEntity != null)
                {
                    applicationEntity.ApplicationId = GetCurrentOAuthApplication().Id;
                }
                _entityDbSet.Add(entity);
                _databaseContext.SaveChanges();
                if (reloadNavigationProperties)
                    //reload the entity to load the navigation properties
                    ReloadWithNavigationProperties(entity);
            }
            catch (DbEntityValidationException ex)
            {

            }
        }

        public void Update(T entity)
        {
            _SetupContexts();
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
                _SetupContexts();

                _entityDbSet.Remove(entity);

                _databaseContext.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {

            }
        }
        
        public void Delete(Expression<Func<T, bool>> @where)
        {
            _SetupContexts();

            try
            {
                var entities = _entityDbSet.Where(where).ToList();
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

        private OAuthApplication GetCurrentOAuthApplication()
        {
            var owinContext = ApplicationHelper.GetCurrentOwinContext();
            var application = owinContext.Get<OAuthApplication>(OwinEnvironmentVariableNames.ActiveClient);
            return application;
        }

        private Expression<Func<T, bool>> AppendAdditionalConditions(Expression<Func<T, bool>> where)
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                //the parameter
                var param = Expression.Parameter(typeof(T), "x");
                var deletedWhere =
                    Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(Expression.Property(param, "Deleted"), Expression.Constant(false)), param);

                //combine these to create a single expression
                where = ExpressionHelpers.CombineAnd<T>(where, deletedWhere);
            }

            if (typeof(IPerApplicationEntity).IsAssignableFrom(typeof(T)))
            {
              
                var applicationId = GetCurrentOAuthApplication()?.Id ?? 0;

                //the parameter
                var param = Expression.Parameter(typeof(T), "x");
                var applicationIdWhere =
                    Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(Expression.Property(param, "ApplicationId"), Expression.Constant(applicationId)), param);

                //combine these to create a single expression
                where = ExpressionHelpers.CombineAnd<T>(where, applicationIdWhere);
            }


            return where;
        }

        private void ReloadWithNavigationProperties(T entity)
        {
            var oc = ((IObjectContextAdapter)_databaseContext).ObjectContext;
            //we first detach the entity and get it again from the database to perform the reload
            oc.Detach(entity); //detach entity to retrieve entity with navigation properties
            var refreshedEntity = Get(entity.Id);

            var fields = entity.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var value = field.GetValue(refreshedEntity);
                //assign to the original object
                field.SetValue(entity, value);
            }

            var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(refreshedEntity);
                //assign to the original object
                property.SetValue(entity, value);
            }
        }
    }
}