using Orchard.Environment.Descriptor.Models;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// Service for producing a default <see cref="ShellDescriptor"/> to be used from 
    /// <see cref="Lombiq.OrchardAppHost.Services.TransientHost.TransientShellDescriptorManager"/> when the latter one
    /// is initialized.
    /// </summary>
    public interface IDefaultTransientShellDescriptorProvider
    {
        ShellDescriptor GetDefaultShellDescriptor();
    }


    public class DefaultTransientShellDescriptorProvider : IDefaultTransientShellDescriptorProvider
    {
        private readonly ShellDescriptor _defaultShellDescriptor;


        public DefaultTransientShellDescriptorProvider(ShellDescriptor defaultShellDescriptor)
        {
            _defaultShellDescriptor = defaultShellDescriptor;
        }
        

        public ShellDescriptor GetDefaultShellDescriptor()
        {
            return _defaultShellDescriptor;
        }
    }
}
