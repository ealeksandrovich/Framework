namespace Framework.DataProviders.NHibernate
{
    using System;
    using global::NHibernate;
    using global::NHibernate.Context;

    public class NhSessionContext: CurrentSessionContext
    {
        #region [Constants]

        /// <summary>
        /// The http session key.
        /// </summary>
        private const string HttpSessionKey = "E6CAE093-DE8E-4E46-BCBA-F8D75245B610";

        #endregion

        #region [Fields]

        /// <summary>
        /// The thread session.
        /// </summary>
        [ThreadStatic]
        private static ISession threadSession;

        #endregion

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        protected override ISession Session
        {
            get
            {
                var currentContext = ReflectiveHttpContext.HttpContextCurrentGetter();
                if (currentContext != null)
                {
                    var items = ReflectiveHttpContext.HttpContextItemsGetter(currentContext);
                    return items[HttpSessionKey] as ISession;
                }

                return threadSession;
            }

            set
            {
                var currentContext = ReflectiveHttpContext.HttpContextCurrentGetter();
                if (currentContext != null)
                {
                    var items = ReflectiveHttpContext.HttpContextItemsGetter(currentContext);
                    items[HttpSessionKey] = value;
                    return;
                }

                threadSession = value;
            }
        }
    }
}
