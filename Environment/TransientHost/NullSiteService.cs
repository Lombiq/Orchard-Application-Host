using Orchard.Environment.Extensions;
using Orchard.Settings;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// Used in transient hosts to prevent exceptions caused by instantiating the default <see cref="ISiteService"/>
    /// implementation due to transient hosts not having persistence configured.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class NullSiteService : ISiteService
    {
        public ISite GetSiteSettings()
        {
            return null;
        }
    }
}
