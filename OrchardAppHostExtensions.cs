using System;
using System.Threading.Tasks;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Configuration;

namespace Lombiq.OrchardAppHost
{
    public static class OrchardAppHostExtensions
    {
        public static Task Run(this IOrchardAppHost appHost, Func<IWorkContextScope, Task> process)
        {
            return appHost.Run(process, ShellSettings.DefaultName);
        }

        public static Task RunInTransaction(this IOrchardAppHost appHost, Func<IWorkContextScope, Task> process)
        {
            return appHost.RunInTransaction(process, ShellSettings.DefaultName);
        }

        public static Task RunInTransaction(this IOrchardAppHost appHost, Func<IWorkContextScope, Task> process, string shellName)
        {
            return appHost.Run(async scope =>
                {
                    var transactionManager = scope.Resolve<ITransactionManager>();
                    transactionManager.Demand();
                    try
                    {
                        await process(scope);
                    }
                    catch (Exception)
                    {
                        transactionManager.Cancel();
                        throw;
                    }
                }, shellName);
        }

        public static Task Run<TService>(this IOrchardAppHost appHost, Func<TService, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2>(this IOrchardAppHost appHost, Func<TService1, TService2, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4, TService5>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, TService5, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4, TService5, TService6>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, TService5, TService6, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4, TService5, TService6, TService7>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, TService5, TService6, TService7, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>(), scope.Resolve<TService7>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>(), scope.Resolve<TService7>(), scope.Resolve<TService8>()), shellName, wrapInTransaction);
        }

        public static Task Run<TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8, TService9>(this IOrchardAppHost appHost, Func<TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8, TService9, Task> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            return appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>(), scope.Resolve<TService7>(), scope.Resolve<TService8>(), scope.Resolve<TService9>()), shellName, wrapInTransaction);
        }

        /// <summary>
        /// Runs a process inside the Orchard App Host that retrieves a value. The method is thread-safe.
        /// </summary>
        public static async Task<TResult> RunGet<TResult>(this IOrchardAppHost appHost, Func<IWorkContextScope, Task<TResult>> getterProcess, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            TResult result = default(TResult);
            Func<IWorkContextScope, Task> process = async scope => { result = await getterProcess(scope); };
            await appHost.Run(scope => process(scope), shellName, wrapInTransaction);
            return result;
        }


        private static Task Run(this IOrchardAppHost appHost, Func<IWorkContextScope, Task> process, string shellName, bool wrapInTransaction)
        {
            if (wrapInTransaction) return appHost.RunInTransaction(process, shellName);
            else return appHost.Run(process, shellName);
        }
    }
}
