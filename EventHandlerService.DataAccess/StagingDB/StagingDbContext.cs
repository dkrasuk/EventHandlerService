using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandlerService.DataAccess.StagingDB
{
    public class StagingDbContext : DbContext
    {
        private readonly string _schema;
        public StagingDbContext(DbConnection conn, bool contextOwnsConnection = true, string schema = "")
            : base(conn, contextOwnsConnection)
        {
            _schema = schema;
        }
        public StagingDbContext()
            : base("name=Staging")
        {
        }

        static StagingDbContext()
        {
            Database.SetInitializer<StagingDbContext>(null);
        }
    }
}
