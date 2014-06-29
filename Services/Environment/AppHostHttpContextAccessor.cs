using System;
using System.Web;
using Orchard.Mvc;

namespace Lombiq.OrchardAppHost.Services.Environment
{
    public class AppHostHttpContextAccessor : IHttpContextAccessor
    {
        private Lazy<HttpContextBase> _httpContextLazy;


        // Lazy is needed because there is a circular dependency: WorkContextAccessor -> this -> HttpContextBase -> WCA...
        public AppHostHttpContextAccessor(Lazy<HttpContextBase> httpContextLazy)
        {
            _httpContextLazy = httpContextLazy;
        }
        
    
        public HttpContextBase Current()
        {
            return _httpContextLazy.Value;
        }

        public void Set(HttpContextBase stub)
        {
            _httpContextLazy = new Lazy<HttpContextBase>(() => stub);
        }
    }
}
