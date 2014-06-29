using Autofac;
using Autofac.Core;
using Orchard.Environment.Descriptor;

namespace Lombiq.OrchardAppHost.Services
{
    public class ShellDescriptorManagerModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (!registration.Activator.LimitType.IsAssignableTo<IShellDescriptorManager>()) return;

            registration.Activating += (sender, e) =>
                {
                    var decorator = new ImportedExtensionsEnablingShellDescriptorManager(
                        (IShellDescriptorManager)e.Instance,
                        e.Context.Resolve<IImportedExtensionsAccessor>());
                    e.Instance = decorator;
                };
        }
    }
}
