using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandlerService.DataAccess
{
    /// <summary>
    /// Contract for classes providing unit of work pattern implementation for data access and editing.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Returns a IQueriable of all data objects of the specified type.
        /// </summary>
        IQueryable<T> Get<T>()
            where T : class;

        /// <summary>
        /// Returns a single object of the specified type.
        /// </summary>
        T GetSingle<T>(Func<T, bool> selector) where T : class;

        /// <summary>
        /// Returns a single object of the specified type from local cache.
        /// </summary>
        T GetSingleLocalOrDatabase<T>(Func<T, bool> selector) where T : class;

        /// <summary>
        /// Returns a IQueriable of all data objects of the specified type while eagerly loading all subproperties which are of one of the specified types.
        /// </summary>
        IQueryable<T> GetEager<T, TInclude>()
            where T : class
            where TInclude : class;

        /// <summary>
        /// Marks object as added in the unit of work.
        /// </summary>
        void Add<T>(T entity)
            where T : class;

        /// <summary>
        /// Marks object as removed in the unit of work.
        /// </summary>
        void Remove<T>(T entity)
            where T : class;

        /// <summary>
        /// Marks many objectes as removed in the unit of work.
        /// </summary>
        void RemoveMany<T>(IEnumerable<T> entity)
            where T : class;

        /// <summary>
        /// Commits current changes.
        /// </summary>
        void Commit();

        /// <summary>
        /// Returns true if the unit of work provides access to objects of the given type.
        /// </summary>
        bool SupportsType(Type type);

        /// <summary>
        /// Return a IEnumerable of the data objects of the specified type by sql query. 
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="sql">Sql to execute</param>
        /// <param name="parameters">Query parameters collection (SqlParameter's)</param>
        /// <returns>Collection of the T type</returns>
        IQueryable<T> ExecuteQuery<T>(string sql, params object[] parameters);

        IEnumerable<DbParameter> ExecuteQuery(string sql, params object[] parameters);
    }
}
