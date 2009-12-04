using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Context;
using System.Web;
using NHibernate;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.StaticFactory;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using System.Web.Routing;
using System.Web.Mvc;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using Ozmosis.NHibernateImpl;
using NHibernate.Tool.hbm2ddl;

namespace Ozmosis
{
    public abstract class OzmosisApplication : HttpApplication
    {
        public OzmosisApplication()
        {
            Bootstrap();
        }

        protected abstract string ConnectionStringName { get; }
        protected abstract Type SampleEntityType { get; }
        protected abstract Func<Type, bool> EntityFilter { get; }

        private void Bootstrap()
        {
            var container = new UnityContainer();
            container.AddNewExtension<StaticFactoryExtension>();

            RegisterNHibernateStuff(container);
            RegisterCRUDStuff(container);
            RegisterRouteRegistrars(container);

            var serviceLocator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            var registrars = ServiceLocator.Current.GetAllInstances<IRouteRegistrar>();
            foreach (var registrar in registrars)
                registrar.RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new CommonServiceLocatorControllerFactory());
        }

        #region Container Configuration

        private void RegisterNHibernateStuff(IUnityContainer container)
        {
            container.RegisterInstance<ISessionFactory>(CreateSessionFactory());
            container.RegisterType<ISession>(new RequestContextLifeTimeManager(typeof(ISession)));

            container.Configure<StaticFactoryExtension>()
                     .RegisterFactory<ISession>(c => c.Resolve<ISessionFactory>().GetCurrentSession());

            container.RegisterType<IRepository, NHibernateRepository>();
            container.RegisterType<IUnitOfWorkProvider, NHibernateUnitOfWorkProvider>();
        }

        private void RegisterCRUDStuff(IUnityContainer container)
        {
            var entityTypes = SampleEntityType.Assembly.GetTypes()
                                   .Where(x => typeof(IEntity).IsAssignableFrom(x) && x.IsAbstract == false);

            foreach (var entityType in entityTypes)
            {
                container.RegisterType(
                    typeof(IController),
                    typeof(CRUDController<>).MakeGenericType(entityType),
                    entityType.Name.ToLower()
                );
            }
        }

        private static void RegisterRouteRegistrars(IUnityContainer container)
        {
            var registrarTypes = typeof(OzmosisApplication).Assembly.GetTypes()
                                    .Where(x => typeof(IRouteRegistrar).IsAssignableFrom(x) && x.IsAbstract == false);

            foreach (var registrarType in registrarTypes)
            {
                container.RegisterType(typeof(IRouteRegistrar), registrarType, registrarType.Name.ToLower());
            }
        }

        #endregion

        #region Session Factory

        private ISessionFactory CreateSessionFactory()
        {
            var Database = MsSqlConfiguration.MsSql2008
                .ConnectionString(conn =>
                    conn.FromConnectionStringWithKey(ConnectionStringName));

            return Fluently.Configure()
                .Database(Database)
                .Mappings(CreateMappings)
                .ExposeConfiguration(SetupOpenSessionInView)
                .ExposeConfiguration(UpdateSchema)
                .BuildSessionFactory();
        }

        private void CreateMappings(MappingConfiguration m)
        {
            var model = AutoMap.Assembly(SampleEntityType.Assembly, EntityFilter);
            m.AutoMappings.Add(model);
        }

        private static void SetupOpenSessionInView(NHibernate.Cfg.Configuration cfg)
        {
            cfg.SetProperty("current_session_context_class", "managed_web");
        }

        private static void UpdateSchema(
            NHibernate.Cfg.Configuration cfg
        )
        {
            new SchemaUpdate(cfg)
                          .Execute(false, true);
        }

        #endregion

        #region Session Management

        public void Application_BeginRequest(object sender, EventArgs e)
        {
            ManagedWebSessionContext.Bind(
                HttpContext.Current,
                ServiceLocator.Current.GetInstance<ISessionFactory>().OpenSession()
            );
        }

        public void Application_EndRequest(object sender, EventArgs e)
        {
            ISession session = ManagedWebSessionContext.Unbind(
                HttpContext.Current, ServiceLocator.Current.GetInstance<ISessionFactory>());
            if (session != null)
            {
                if (session.Transaction != null &&
                    session.Transaction.IsActive)
                {
                    session.Transaction.Rollback();
                }
                else
                    session.Flush();
                session.Close();
            }
        }

        #endregion
    }
}
