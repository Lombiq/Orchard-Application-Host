using Orchard.Data;
using Orchard.Environment.Extensions;
using System;

namespace Lombiq.OrchardAppHost.Environment.TransientHost
{
    /// <summary>
    /// The default <see cref="ITransactionManager"/> implementation would fail because of transient hosts not using
    /// database access. Thus we need so e.g. background tasks run.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.Data.SessionLocator")]
    public class NullSessionLocator : ISessionLocator
    {
        public NHibernate.ISession For(Type entityType)
        {
            throw new NotImplementedException();
        }
    }
}
