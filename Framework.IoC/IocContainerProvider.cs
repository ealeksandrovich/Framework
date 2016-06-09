namespace Framework.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using SimpleInjector;

    public class IocContainerProvider
    {
        /// <summary>
        /// Sync object
        /// </summary>
        private static readonly object LockObj = new object();

        /// <summary>
        /// Current DI container. 
        /// </summary>
        public static Container CurrentContainer { get; private set; }

        /// <summary>
        /// Initialize container from <see cref="IIocModule"/> modules defined in a solution.
        /// </summary>
        public static void InitIoc()
        {
            var container = GetContainer();

            // we need to sync it despite the fact of static member because Registering of objects is not thread safe operation.
            lock (LockObj)
            {
                List<Type> list = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).ToList();
                foreach (
                    Type module in
                        list.Where(x => x.GetInterfaces().Contains(typeof (IIocModule))).DistinctBy(x => x.FullName))
                {
                    InitModule(module, container);
                }
            }

            CurrentContainer = container;
        }

        public static void InitIoc(List<Type> modules)
        {
            var container = GetContainer();
            lock (LockObj)
            {
                foreach (
                    Type module in
                        modules.Where(x => x.GetInterfaces().Contains(typeof (IIocModule))).DistinctBy(x => x.FullName))
                {
                    InitModule(module, container);
                }
            }

            CurrentContainer = container;
        }

        #region Private methods

        private static Container GetContainer()
        {
            var container = new Container();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.SuppressLifestyleMismatchVerification = true;
            return container;
        }

        private static void InitModule(Type module, Container container)
        {
            Trace.WriteLine(string.Format("IoC module found:{0} {1}", module.FullName, module.Assembly.FullName));

            var callingMethod = Expression.Call(Expression.New(module), module.GetMethod("Register"),
                Expression.Constant(container, typeof (Container)));

            Expression.Lambda<Action>(callingMethod).Compile().Invoke();
        }

        #endregion
    }
}