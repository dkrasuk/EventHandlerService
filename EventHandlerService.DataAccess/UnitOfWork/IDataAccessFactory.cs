using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHandlerService.DataAccess
{
    public interface IDataAccessFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
