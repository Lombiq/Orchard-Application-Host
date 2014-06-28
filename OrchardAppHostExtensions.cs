using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lombiq.OrchardAppHost.Services;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Orchard.Exceptions;
using Orchard.Logging;

namespace Lombiq.OrchardAppHost
{
    public static class OrchardAppHostExtensions
    {
        public static void Run(this IOrchardAppHost appHost, Action<IWorkContextScope> process)
        {
            appHost.Run(process, ShellSettings.DefaultName);
        }

        public static void RunInTransaction(this IOrchardAppHost appHost, Action<IWorkContextScope> process)
        {
            appHost.RunInTransaction(process, ShellSettings.DefaultName);
        }

        public static void RunInTransaction(this IOrchardAppHost appHost, Action<IWorkContextScope> process, string shellName)
        {
            appHost.Run(scope =>
                {
                    var transactionManager = scope.Resolve<ITransactionManager>();
                    transactionManager.Demand();
                    try
                    {
                        process(scope);
                    }
                    catch (Exception)
                    {
                        transactionManager.Cancel();
                        throw;
                    }
                }, shellName);
        }

        public static void Run<TService>(this IOrchardAppHost appHost, Action<TService> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2>(this IOrchardAppHost appHost, Action<TService1, TService2> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2, TService3>(this IOrchardAppHost appHost, Action<TService1, TService2, TService3> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2, TService3, TService4>(this IOrchardAppHost appHost, Action<TService1, TService2, TService3, TService4> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2, TService3, TService4, TService5>(this IOrchardAppHost appHost, Action<TService1, TService2, TService3, TService4, TService5> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2, TService3, TService4, TService5, TService6>(this IOrchardAppHost appHost, Action<TService1, TService2, TService3, TService4, TService5, TService6> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>()), shellName, wrapInTransaction);
        }

        public static void Run<TService1, TService2, TService3, TService4, TService5, TService6, TService7>(this IOrchardAppHost appHost, Action<TService1, TService2, TService3, TService4, TService5, TService6, TService7> process, string shellName = ShellSettings.DefaultName, bool wrapInTransaction = true)
        {
            appHost.Run(scope => process(scope.Resolve<TService1>(), scope.Resolve<TService2>(), scope.Resolve<TService3>(), scope.Resolve<TService4>(), scope.Resolve<TService5>(), scope.Resolve<TService6>(), scope.Resolve<TService7>()), shellName, wrapInTransaction);
        }


        private static void Run(this IOrchardAppHost appHost, Action<IWorkContextScope> process, string shellName, bool wrapInTransaction)
        {
            if (wrapInTransaction) appHost.RunInTransaction(process, shellName);
            else appHost.Run(process, shellName);
        }
    }
}
