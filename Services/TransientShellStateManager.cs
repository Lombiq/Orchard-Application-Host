using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Environment.State;
using Orchard.Environment.State.Models;

namespace Lombiq.OrchardAppHost.Services
{
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class TransientShellStateManager : IShellStateManager, ISingletonDependency
    {
        private const string StoreKey = "Lombiq.OrchardAppHost.TransientShellStateManager.ShellState";

        private readonly ITransientStore _transientStore;

        private ShellState ShellState
        {
            get { return _transientStore.Get<ShellState>(StoreKey); }
            set { _transientStore.Set(StoreKey, value); }
        }


        public TransientShellStateManager(ITransientStore transientStore)
        {
            _transientStore = transientStore;
        }


        public ShellState GetShellState()
        {
            return ShellState ?? new ShellState();
        }

        public void UpdateEnabledState(ShellFeatureState featureState, ShellFeatureState.State value)
        {
            GetOrAddFeatureState(featureState.Name).EnableState = value;

            featureState.EnableState = value;
        }

        public void UpdateInstalledState(ShellFeatureState featureState, ShellFeatureState.State value)
        {
            GetOrAddFeatureState(featureState.Name).InstallState = value;

            featureState.InstallState = value;
        }


        private ShellFeatureState GetOrAddFeatureState(string featureName)
        {
            var shellState = GetShellState();

            var feature = shellState.Features.SingleOrDefault(f => f.Name == featureName);
            if (feature == null)
            {
                feature = new ShellFeatureState { Name = featureName };
                shellState.Features = shellState.Features.Union(new[] { feature });
            }

            ShellState = shellState;

            return feature;
        }
    }
}
