using NHibernate;
using Orchard.Data;
using Orchard.Environment.Extensions;
using System;
using System.Data;

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
