using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using MvcMusicStore.Controllers;
using MvcMusicStore.Factories;
using MvcMusicStore.Models;
using MvcMusicStore.Services;
using MvcMusicStore.Filters;

namespace MvcMusicStore
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Use LocalDB for Entity Framework by default
            Database.DefaultConnectionFactory = new SqlConnectionFactory("Data Source=(localdb)\v11.0; Integrated Security=True; MultipleActiveResultSets=True");

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var container = new UnityContainer();
            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IController, StoreController>("Store");

            var factory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(factory);

            container.RegisterInstance<IMessageService>(new MessageService { 
                Message = "You are welcome to our Web Camps Training Kit!",
                ImageUrl = "/Content/Images/webcamps.png"
            });

            container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));

            container.RegisterInstance<IFilterProvider>("FilterProvider", new FilterProvider(container));
            container.RegisterInstance<IActionFilter>("LogActionFilter", new TraceActionFilter());

            IDependencyResolver resolver = DependencyResolver.Current;

            IDependencyResolver newResolver = new UnityDependencyResolver(container, resolver);

            DependencyResolver.SetResolver(newResolver);

            BundleTable.Bundles.RegisterTemplateBundles();

            // Clean up Logs Table
            MusicStoreEntities storeDB = new MusicStoreEntities();
            storeDB.ExecuteStoreCommand("TRUNCATE TABLE [ActionLog]");
        }
    }
}