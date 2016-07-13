using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Environment.TransientHost
{
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.Data.TransactionManager")]
    public class NullTransactionManager : ITransactionManager
    {
        public void Cancel()
        {
        }

        public void Demand()
        {
        }

        public ISession GetSession()
        {
            throw new NotImplementedException();
        }

        public void RequireNew()
        {
        }

        public void RequireNew(IsolationLevel level)
        {
        }
    }
}
