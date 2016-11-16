namespace Framework.WebApi
{
    using System;
    using System.Configuration;
    using Configuration;
    using NLog;
    using Topshelf;

    public class ServiceContainer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Run<TService>(Func<TService> serviceFactory) 
            where TService : OwinService
        {
            if (ServiceContainerSection.Current == null)
            {
                throw new ConfigurationErrorsException(
                    "Configuration section \"serviceContainer\" hasn't beent found. ServiceContainer cannnot be runed.");
            }

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            Run(serviceFactory, ServiceContainerSection.Current);
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Fatal("Unhandled exception occured {0}", e.ExceptionObject);
        }

        public static void Run<TService>(Func<TService> serviceFactory, ServiceContainerSection configuration) where TService : OwinService
        {
            try
            {
                HostFactory.Run(
                    x =>
                    {
                        x.UseNLog();
                        x.Service<TService>(
                            sc =>
                            {
                                sc.ConstructUsing(serviceFactory);
                                // the start and stop methods for the service
                                sc.WhenStarted(s => s.Start());
                                sc.WhenStopped(s => s.Stop());

                                // optional, when shutdown is supported
                                sc.WhenShutdown(s => s.Shutdown());
                            });
                        if (!string.IsNullOrEmpty(configuration.ServiceName))
                        {
                            x.SetServiceName(configuration.ServiceName);
                        }
                        if (!string.IsNullOrEmpty(configuration.Description))
                        {
                            x.SetDescription(configuration.Description);
                        }
                        if (!string.IsNullOrEmpty(configuration.DisplayName))
                        {
                            x.SetDisplayName(configuration.DisplayName);
                        }
                        if (!string.IsNullOrEmpty(configuration.InstanceName))
                        {
                            x.SetInstanceName(configuration.InstanceName);
                        }

                        x.EnableShutdown();
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error occured during starting of the service");
                throw;
            }
        }
    }
}
