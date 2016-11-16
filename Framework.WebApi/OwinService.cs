namespace Framework.WebApi
{
    using System;
    using System.Configuration;
    using IoC;
    using Microsoft.Owin.Hosting;
    using NLog;

    public class OwinService
    {
        private IDisposable owinWeb;

        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public virtual void Start()
        {
            this.logger.Info("IoC initializing....");
            IocContainerProvider.InitIoc();
            IocContainerProvider.CurrentContainer.Verify();
            this.logger.Info("IoC initialized.");

            this.logger.Info("Service starting");
            var serviceUrl = ConfigurationManager.AppSettings["serviceUrl"];
            this.owinWeb = WebApp.Start<Startup>(serviceUrl);
            this.logger.Info("Service started");
        }

        public virtual void Shutdown()
        {
            this.logger.Info("Service shutdown");
        }

        public virtual void Stop()
        {
            this.owinWeb.Dispose();
            this.logger.Info("Service stopped");
        }
    }
}
