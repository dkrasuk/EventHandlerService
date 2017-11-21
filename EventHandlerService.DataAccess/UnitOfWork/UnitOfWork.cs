using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace EventHandlerService.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dataContext;

        public UnitOfWork(DbContext dataContext)
        {
            _dataContext = dataContext;
            _dataContext.Configuration.LazyLoadingEnabled = true;
        }

        #region Implementation of IUnitOfWork

        /// <summary>
        /// Return a IQueryable of all data objects of the specified type.
        /// </summary>
        public IQueryable<T> Get<T>()
            where T : class
        {
            return _dataContext.Set<T>();
        }

        public T GetSingle<T>(Func<T, bool> selector) where T : class
        {
            return _dataContext.Set<T>().FirstOrDefault(selector);
        }

        public T GetSingleLocalOrDatabase<T>(Func<T, bool> selector) where T : class
        {
            var localData = _dataContext.Set<T>().Local.FirstOrDefault(selector);
            var dbData = _dataContext.Set<T>().FirstOrDefault(selector);
            return localData ?? dbData;
        }

        /// <summary>
        /// Returns a IQueriable of all data objects of the specified type while eagerly loading all subproperties which are of one of the specified types.
        /// </summary>
        public IQueryable<T> GetEager<T, TInclude>()
            where T : class
            where TInclude : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks object as added in the unit of work.
        /// </summary>
        public void Add<T>(T entity)
            where T : class
        {
            _dataContext.Set<T>().Add(entity);
        }

        /// <summary>
        /// Marks object as removed in the unit of work.
        /// </summary>
        public void Remove<T>(T entity)
            where T : class
        {
            if (entity != null)
                _dataContext.Set<T>().Remove(entity);
        }

        /// <summary>
        /// Marks nmany objectes as removed in the unit of work.
        /// </summary>
        public void RemoveMany<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null || !entities.Any())
                return;

            foreach (var entity in entities)
                Remove(entity);
        }

        /// <summary>
        /// Commits current changes.
        /// </summary>
        public void Commit()
        {
            _dataContext.SaveChanges();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Cleans up external resources.
        /// </summary>
        public void Dispose()
        {
            if (_dataContext != null)
                _dataContext.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Returns true if data context has the entity of current type, otherwise - false.
        /// </summary>
        public bool SupportsType(Type type)
        {
            var adapter = _dataContext as IObjectContextAdapter;

            ObjectContext context = adapter.ObjectContext;
            EntityContainer container = context.MetadataWorkspace
               .GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            var result = container.BaseEntitySets.OfType<EntitySet>().FirstOrDefault(es => es.ElementType.Name == type.Name);

            return result != null;
        }

        /// <summary>
        /// Return a IEnumerable of the data objects of the specified type by sql query. 
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="sql">Sql to execute</param>
        /// <param name="parameters">Query parameters collection (SqlParameter's)</param>
        /// <returns>Collection of the T type</returns>
        public IQueryable<T> ExecuteQuery<T>(string sql, params object[] parameters)
        {
            var result = _dataContext.Database.SqlQuery<T>(sql, parameters).AsQueryable();
            return result;
        }

        public IEnumerable<DbParameter> ExecuteQuery(string sql, params object[] parameters)
        {

            try
            {
                _dataContext.Database.ExecuteSqlCommand(sql, parameters);
               
            }
            catch (Exception e )
            {
                var a = e.Message;
            }  
            


            return
                (from o in parameters.OfType<DbParameter>() where o.Direction == ParameterDirection.Output select o)
                    .ToList();
        }
    }
}
