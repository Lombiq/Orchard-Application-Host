using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
