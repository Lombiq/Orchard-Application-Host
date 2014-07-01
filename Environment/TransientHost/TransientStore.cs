using System.Collections.Concurrent;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// A service that stores data for the life time of the Orchard Application Host.
    /// </summary>
    public interface IHostTransientStore
    {
        T Get<T>(string key);
        void Set(string key, object value);
    }


    // Consumers can see stale data in a concurrent application...
    public class HostTransientStore : IHostTransientStore
    {
        private readonly ConcurrentDictionary<string, object> _store = new ConcurrentDictionary<string,object>();

        
        public T Get<T>(string key)
        {
            return _store.ContainsKey(key) ? (T)_store[key] : default(T);
        }

        public void Set(string key, object value)
        {
            _store[key] = value;
        }
    }


    /// <summary>
    /// A service that stores data for the life time of the Orchard Application Host for a given shell.
    /// </summary>
    public interface ITransientStore : IHostTransientStore, ISingletonDependency
    {
    }


    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class TransientStore : ITransientStore
    {
        private readonly IHostTransientStore _hostTransientStore;
        private readonly ShellSettings _shellSettings;


        public TransientStore(IHostTransientStore hostTransientStore, ShellSettings shellSettings)
        {
            _hostTransientStore = hostTransientStore;
            _shellSettings = shellSettings;
        }
        
    
        public T Get<T>(string key)
        {
            return _hostTransientStore.Get<T>(MakeKey(key));
        }

        public void Set(string key, object value)
        {
            _hostTransientStore.Set(MakeKey(key), value);
        }


        private string MakeKey(string key)
        {
            return _shellSettings.Name + "." + key;
        }
    }
}
