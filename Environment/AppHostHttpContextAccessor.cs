using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using Autofac;
using Autofac.Core;
using Orchard;
using Orchard.Mvc;

namespace Lombiq.OrchardAppHost.Environment
{
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
