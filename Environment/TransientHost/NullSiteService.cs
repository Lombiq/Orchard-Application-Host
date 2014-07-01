using Orchard.Environment.Extensions;
using Orchard.Settings;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class NullSiteService : ISiteService
    {
        public ISite GetSiteSettings()
        {
            return null;
        }
    }
}
