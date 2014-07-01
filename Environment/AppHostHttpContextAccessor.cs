using System.Collections;
using System.Collections.Generic;
using System.Web;
using Orchard.Mvc;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IHttpContextAccessor"/> implementation for the Orchard App Host. It's used to carry an <see cref="HttpContextBase"/>
    /// instance throughout the scope of processes run in the App Host. This is necessary to maintain the work context throughout the 
    /// process, even if it's async (and thus causes thread switches).
    /// </summary>
    public class AppHostHttpContextAccessor : IHttpContextAccessor
    {
        private HttpContextBase _stub = null;


        public HttpContextBase Current()
        {
            return _stub;
        }

        public void Set(HttpContextBase stub)
        {
            _stub = stub;
        }


        public class HttpContextPlaceholder : HttpContextBase
        {
            private readonly IDictionary _items = new Dictionary<object, object>();

            public override IDictionary Items
            {
                get { return _items; }
            }
        }
    }
}
