using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Data;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Environment.TransientHost
{
    /// <summary>
    /// The default <see cref="ITransactionManager"/> implementation would fail because of transient hosts not using
    /// database access. Thus we need so e.g. background tasks run.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.Data.SessionLocator")]
    public class NullSessionLocator : ISessionLocator, ITransactionManager
    {
        public NHibernate.ISession For(Type entityType)
        {
            throw new NotImplementedException();
        }

        public void Demand()
        {
        }

        public void RequireNew()
        {
        }

        public void RequireNew(System.Data.IsolationLevel level)
        {
        }

        public void Cancel()
        {
        }

        public NHibernate.ISession GetSession()
        {
            throw new NotImplementedException();
        }
    }
}
