using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc3;
using MvcMusicStore.Services;
using MvcMusicStore.Controllers;

namespace MvcMusicStore
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IController, StoreController>("Store");        

            return container;
        }
    }
}