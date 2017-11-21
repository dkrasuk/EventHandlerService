using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandlerService.DataAccess
{
    public class DataAccessFactory<T>: IDataAccessFactory
    {
        private readonly DbConnection _dbConnection;
        private readonly bool _contextOwnsConnection;
        private readonly string _schema;

        public DataAccessFactory(DbConnection dbConnection, bool contextOwnsConnection = false, string schema = "")
        {
            _dbConnection = dbConnection;
            _contextOwnsConnection = contextOwnsConnection;
            _schema = schema;
        }
        public IUnitOfWork CreateUnitOfWork()
        {
            var context = (DbContext)Activator.CreateInstance(typeof(T), _dbConnection, _contextOwnsConnection, _schema);        
            return new UnitOfWork(context);
        }
    }
}
