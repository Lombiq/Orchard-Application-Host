using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.State;

namespace Lombiq.OrchardAppHost.Environment
{
    public interface IProcessingEngineTask
    {
        string MessageName { get; }
        Dictionary<string, object> Parameters { get; }
        ShellDescriptor ShellDescriptor { get; }
        ShellSettings ShellSettings { get; }
    }


    /// <summary>
    /// Service for detecting and registering when tasks are added to <see cref="IProcessingEngine"/>. These tasks are 
    /// then explicitly queued again. This is necessary as <see cref="IProcessingEngine"/>'s implementation looses 
    /// registered tasks on thread switches (what happens in async code).
    /// </summary>
    public interface IProcessingEngineTaskAddedHandler
    {
        IEnumerable<IProcessingEngineTask> GetAddedTasks();
    }


    public class ProcessingEngineTaskAddedHandler : IProcessingEngineTaskAddedHandler, IProcessingEngine
    {
        private readonly ConcurrentBag<IProcessingEngineTask> _addedTasks = new ConcurrentBag<IProcessingEngineTask>();


        IEnumerable<IProcessingEngineTask> IProcessingEngineTaskAddedHandler.GetAddedTasks()
        {
            return _addedTasks;
        }

        string IProcessingEngine.AddTask(ShellSettings shellSettings, ShellDescriptor shellDescriptor, string messageName, Dictionary<string, object> parameters)
        {
            _addedTasks.Add(new ProcessingEngineTask
                {
                    ShellSettings = shellSettings,
                    ShellDescriptor = shellDescriptor,
                    MessageName = messageName,
                    Parameters = parameters
                });

            // As in DefaultProcessingEngine.
            return Guid.NewGuid().ToString("n");
        }

        bool IProcessingEngine.AreTasksPending()
        {
            return _addedTasks.Any();
        }

        void IProcessingEngine.ExecuteNextTask()
        {
            throw new NotImplementedException();
        }


        public class ProcessingEngineTask : IProcessingEngineTask
        {
            public ShellSettings ShellSettings { get; set; }
            public ShellDescriptor ShellDescriptor { get; set; }
            public string MessageName { get; set; }
            public Dictionary<string, object> Parameters { get; set; }
        }
    }
}
