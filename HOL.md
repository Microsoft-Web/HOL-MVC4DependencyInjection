
#ASP.NET MVC - Dependency Injection#
---

## Overview ##

**Note:** This Hands-on Lab assumes you have basic knowledge of **ASP.NET MVC** and **ASP.NET MVC 4 filters**. If you have not used **ASP.NET MVC 4 filters** before, we recommend you to go over **ASP.NET MVC Custom Action Filters** and **MVC Global and Dynamic Action filters** Hand-on Lab.

In Object Oriented Programming paradigm, objects work together in a collaboration model where there are contributors and consumers. Naturally, this communication model generates dependencies between objects and components that could became difficult to manage when complexity increases .

 ![Class dependencies and model complexity](./images/Class_dependencies_and_model_complexity.png?raw=true "Class dependencies and model complexity")
 
_Class dependencies and model complexity_

You have probably heard about the Factory Pattern and the separation between the interface and the implementation using services. However, the client objects are often responsible for service location.

Before introducing the Dependency Injection Pattern, we will explain what Inversion of Control (IoC) principle is.

With _**Inversion of Control (Ioc)**_, consumer objects do not create the other objects on which they rely. Those objects come from an external source.

### The Dependency Injection (DI) Design Pattern ###

Dependency injection (DI) design pattern is based on separating component behavior from dependency resolution without object intervention.

This pattern is a particular implementation of Inversion of Control, where the consumer object receives his dependencies inside constructor properties or arguments.

DI requires a framework component behind to deal with class constructor.

 ![Overview - Dependency Injection diagram](./images/Overview_Dependency_Injection_diagram.png?raw=true "Overview - Dependency Injection diagram")
 
_Overview - Dependency Injection diagram_

The advantages of using Dependency Injection pattern and Inversion of Control are the following:

- Reduces  class coupling

- Increases code reusing

- Improves code maintainability

- Improves application testing

 
> **Note:** Depencency Injection is sometimes compared with Abstract Factory Design Pattern, but there is a slight difference between both approaches. DI has a Framework working behind to solve dependencies by calling the factories and the registered services.

Now that you understand the Dependency Injection Pattern, you will learn through this lab how to apply it on ASP.NET MVC 4. You will start using Dependency Injection on **Controllers** to include a service for database access. Next you will use Dependency Injection on **Views** to use a service inside a view and display information. Then, you will extend DI to MVC 4 Filters concept and inject a **Custom Action Filter** in the solution.

In this Hands-on Lab, you will learn how to:

- Integrate MVC 4 with Unity Application Block for Dependency Injection

- Use dependency injection inside an MVC Controller

- Use dependency injection inside an MVC View

- Use dependency injection inside an MVC Action Filter

 
> **Note:** This Lab proposes Unity Application Block as the dependency resolver framework, but it is posible to adapt any Dependency Injection Framework to work with MVC 4.

### System Requirements ###

You must have the following items to complete this lab:

- Visual Studio 11 Express Beta for Web

	**Note:** NEEDS UPDATE You can install the previous system requirements by using the Web Platform Installer 3.0: [http://go.microsoft.com/fwlink/?LinkID=194638](http://go.microsoft.com/fwlink/?LinkID=194638).

 
## Exercises ##

This Hands-On Lab is comprised by the following exercises:

1. Exercise 1: Injecting a Controller

1. Exercise 2: Injecting a View

1. Exercise 3: Injecting Filters

 
Estimated time to complete this lab: **30 minutes**.

> **Note:** Each exercise is accompanied by an **End** folder containing the resulting solution you should obtain after completing the exercises. You can use this solution as a guide if you need additional help working through the exercises.

### Exercise 1: Injecting a Controller ###

In this exercise, you will learn how to use Dependency Injection in MVC Controllers, by integrating Unity Application Block. For that reason you will include services into your MVC Music Store controllers to separate the logic from the data access. The service will create a new dependence into the controller constructor that will be resolved using Dependency Injection with the help of **Unity** application block.

With this approach you will learn how to generate less coupled applications, which are more flexible and easier to maintain and test. Additionally, you will also learn how to integrate MVC with Unity.

### About StoreManager Service ###

The MVC Music Store provided in the begin solution now includes a service that manages the Store Controller data, **StoreService**. Below you will find the Store Service implementation. Note that all the methods return Model entities.

````C#-StoreService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMusicStore.Models;

namespace MvcMusicStore.Services
{
    public class StoreService : MvcMusicStore.Services.IStoreService
    {
        MusicStoreEntities storeDB = new MusicStoreEntities();

        public IList<string> GetGenreNames()
        {
            var genres = from genre in storeDB.Genres
                         select genre.Name;

            return genres.ToList();
        }

        public Genre GetGenreByName(string name)
        {
            var genre = storeDB.Genres.Include("Albums")
                    .Single(g => g.Name == name);
            return genre;
        }

        public Album GetAlbum(int id)
        {
            var album = storeDB.Albums.Single(a => a.AlbumId == id);

            return album;
        }
    }
}
````

Additionally, in the **StoreController** you will find in the begin solution now uses **StoreService**. All data references were removed from Store Controller, and therefore it is possible to modify the current data access provider without making changes at any method that consumes the Store Service.

You will find below that the **Store Controller** implementation has a dependency with the Store Service inside the class constructor.

> **Note:** The dependency introduced in this exercise is related to **MVC Inversion of Control** (IoC).

> The **StoreController** class constructor receives an **IStoreService** parameter, which is essential to perform service calls inside the class. However, **StoreController** does not implement the default constructor (with no parameters) that any controller must have to work with IoC.

> To resolve the dependency, the controller should be created by an abstract factory (a class that returns any object of the specified type).

````C#-StoreController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcMusicStore.ViewModels;
using MvcMusicStore.Models;
using MvcMusicStore.Services;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private IStoreService service;

        public StoreController(IStoreService service)
        {
            this.service = service;
        }

        //
        // GET: /Store/
        public ActionResult Index()
        {
            // Create list of genres
            var genres = this.service.GetGenreNames();

            // Create your view model
            var viewModel = new StoreIndexViewModel
            {
                Genres = genres.ToList(),
                NumberOfGenres = genres.Count()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/Browse?genre=Disco
        public ActionResult Browse(string genre)
        {
            var genreModel = this.service.GetGenreByName(genre);

            var viewModel = new StoreBrowseViewModel()
            {
                Genre = genreModel,
                Albums = genreModel.Albums.ToList()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/Details/5
        public ActionResult Details(int id)
        {
            var album = this.service.GetAlbum(id);

            return View(album);
        }
    }
}
````

**Note:** You will get an error when a class tries to create this Store Controller without sending the service interface, because there is not a parameterless constructor declared.
Through this lab you will learn how to deal with this problem using Dependency Injection with Unity.

 
#### Task 1 - Running the Application ####

In this task, you will run the Begin application, which is now including the service into the Store Controller that separates the data access from the application logic.

After browsing to the store you will receive an exception since the controller service is not passed as a parameter by default:

1. Open the begin solution **MvcMusicStore.sln** at **Source\Ex01-Injecting Controller\Begin**.

1. Press **F5** to run the application.

1. Browse to **/Store** to load Store Controller. You will get the error message "**No parameterless constructor defined for this object**":

 	![Error while running MVC Begin Application](./images/Error_while_running_MVC_Begin_Application.png?raw=true "Error while running MVC Begin Application")
 
	_Error while running MVC Begin Application_

1. Close the browser.

In the following steps you will work on the Music Store Solution to inject the dependency this controller needs.

 
#### Task 2 - Including Unity into MvcMusicStore Solution ####

In this task, you will include NEEDS UPDATE Unity Application Block 2.0 into your solution.

> **Note:** The Unity Application Block (Unity) is a lightweight, extensible dependency injection container with optional support for instance and type interception. It's a general-purpose container for use in any type of .NET application. It provides all the common features found in dependency injection mechanisms including: object creation, abstraction of requirements by specifying dependencies at runtime and flexibility, be deferring the component configuration to the container.

You could read more about Unity 2.0 at [msdn](http://msdn.microsoft.com/en-us/library/ff663144.aspx).

1. Open the begin solution **MvcMusicStore.sln** at **Source\Ex01-Injecting Controller\Begin**.

1. In the **MvcMusicStore** project, add a reference to **Microsoft.Practices.Unity.dll**, which is included in **Source\Assets\Unity 2.0\** folder of this lab.

 
#### Task 3 - Adding a Unity Controller Factory ####

In this task, you will add to the solution a custom controller factory for **Unity**. This class implements **IControllerFactory** interface, extending **CreateController** and **ReleaseController** methods to work with Unity. This factory will create the instances of the controllers that work with Dependency Injection.

**Note:** A controller factory is an implementation of the _IControllerFactory_ interface, which is responsible both for locating a controller type and for instantiating an instance of that controller type. The following implementation of **CreateController** finds the controller by name inside the Unity container and returns an instance if it was found. Otherwise, it delegates the creation of the controller to an inner factory. One of the advantages of this logic is that controllers can be registered by name. You can find **IControllerFactory** interface reference at [msdn](http://msdn.microsoft.com/en-us/library/system.web.mvc.icontrollerfactory.aspx).

1. In the **MvcMusicStore** project, create a new folder named **Factories**, and add the **UnityControllerFactory** class, which is included in the **Source\Assets[C#|VB]** folder of this lab.

	````C#-UnityControllerFactory.cs
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	using System.Web.Routing;
	
	namespace MvcMusicStore.Factories
	{
	    public class UnityControllerFactory : IControllerFactory
	    {
	        private IUnityContainer _container;
	        private IControllerFactory _innerFactory;
	
	        public UnityControllerFactory(IUnityContainer container)
	            : this(container, new DefaultControllerFactory())
	        {
	        }
	
	        protected UnityControllerFactory(IUnityContainer container, IControllerFactory innerFactory)
	        {
	            _container = container;
	            _innerFactory = innerFactory;
	        }
	
	        public IController CreateController(RequestContext requestContext, string controllerName)
	        {
	            try
	            {
	                return _container.Resolve<IController>(controllerName);
	            }
	            catch (Exception)
	            {
	                return _innerFactory.CreateController(requestContext, controllerName);
	            }
	        }
	
	        public void ReleaseController(IController controller)
	        {
	            _container.Teardown(controller);
	        }
	
	        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
	        {
	            return System.Web.SessionState.SessionStateBehavior.Default;
	        }
	    }
	}
	````

	**Note:** This factory class can be reused in any project that uses Dependency Injection for Controllers.

 
#### Task 4 - Registering Unity in Global.asax.cs Application_Start ####

In this task, you will register Unity library into **Global.asax.cs** Application Start.

1. Open **Global.asax.cs** file.

1. Include **Microsoft.Practices.Unity** Application Block, and references to the namespaces **Services**, **Factories** and **Controllers**:

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex1 Injecting Controllers Global using- Csharp)

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using Microsoft.Practices.Unity;
	using MvcMusicStore.Services;
	using MvcMusicStore.Factories;
	using MvcMusicStore.Controllers;
	````

1. Create a new Unity Container in **Global.asax.[cs|vb]** **Application_Start** and register the Store Service and the Store Controller.

	(Code Snippet - ASP.NET MVC Dependency Injection -Ex1 Injecting Controllers Unity Container - Csharp)

	````C#
	...
	protected void Application_Start()
	{
	    AreaRegistration.RegisterAllAreas();
	
	    RegisterGlobalFilters(GlobalFilters.Filters);
	    RegisterRoutes(RouteTable.Routes);
	            
	    var container = new UnityContainer();
	    container.RegisterType<IStoreService, StoreService>();
	    container.RegisterType<IController, StoreController>("Store");
	
	}
	...
	````

1. Register a UnityControllerFactory  of the previous container inside MVC ControllerBuilder as the current factory for the controllers:

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex1 Injecting Controllers Global application start - Csharp)

	````C#
	...
	protected void Application_Start()
	{
	    AreaRegistration.RegisterAllAreas();
	
	    RegisterGlobalFilters(GlobalFilters.Filters);
	    RegisterRoutes(RouteTable.Routes);
	
	           
	    var container = new UnityContainer();
	    container.RegisterType<IStoreService, StoreService>();
	    container.RegisterType<IController, StoreController>("Store");
	
	    var factory = new UnityControllerFactory(container);
	    ControllerBuilder.Current.SetControllerFactory(factory);
	}
	...
	````

	**Note:** **ControllerBuilder** is an MVC class responsible for dynamically building a controller. You can read more about ControllerBuilder at [msdn](http://msdn.microsoft.com/en-us/library/system.web.mvc.controllerbuilder.aspx).

 
#### Task 5 - Running the Application ####

In this task, you will run the application to verify that the Store can now be loaded after including Unity.

1. Press **F5** to run the application.

1. Browse to **/Store**. This will invoke **StoreController**, which is now created by using **UnityControllerFactory**.

 	![MVC Music Store](./images/MVC_Music_Store.png?raw=true "MVC Music Store")
 
	_MVC Music Store_

1. Close the browser.

In the following exercises you will learn how to extend the Dependency Injection scope, and use it inside MVC Views and Action Filters.

### Exercise 2: Injecting a View ###

In this exercise, you will learn how to apply Dependency Injection into a View by using new MVC 4 Features for Unity Integration. In order to do that, you will call a custom service inside the Store Browse View that shows a message with an image below. The service will introduce a new dependency inside the view as it has to be initialized in a point.

Then, you will integrate the project with Unity Application Block and create a Custom Dependency Resolver to inject the dependencies.

#### Task 1 - Creating a View that Consumes a Service ####

In this task, you will create a view that performs a service call to generate a new dependence. The mentioned service is a simple messaging service example that is included in this solution.

1. Open the begin solution **MvcMusicStore.sln** at **Source\Ex02-Injecting View\Begin**.

1. Include **MessageService.cs** and **IMessageService.cs** classes from **Source\Assets** folder inside **/Services** folder.

	**Note:** The **IMessageService** interface defines two properties that are implemented by the **MessageService** class. These properties are **Message** and **ImageUrl** and are defined to hold the message and Url of the image to be displayed.

1. Create the folder **/Pages** at project root, and then add the class **MyBasePage.cs** from **Source\Assets** The base page you will inherit from has the following structure:

(Code Snippet - ASP.NET MVC Dependency Injection  -  Ex02 Injecting Views - MyBasePage - CSharp)	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using Microsoft.Practices.Unity;
	using MvcMusicStore.Services;
	
	namespace MvcMusicStore.Pages
	{
	    public class MyBasePage : System.Web.Mvc.ViewPage<MvcMusicStore.ViewModels.StoreBrowseViewModel>
	    {
	        [Dependency]
	        public IMessageService MessageService { get; set; }
	    }
	}
	````

	> **Note:** On behalf of a technical reason coming from the **ASP.NET** engine, the **dependency** at **IMessageService** interface can't be included into the respective **View Model** Class.

	> The class **MyBasePage** intercepts the relationship between the View and the View-Model so the dependency injection at MessageService can now be inherited by the View.

1. Open **Browse.aspx** view from **/Views/Store** project folder, and make it inherit from **MyBasePage.cs**:

	````C#
	<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" **Inherits="MvcMusicStore.Pages.MyBasePage"** %>
	````

	````VisualBasic
	<%@ Page Title="" Language="vb" MasterPageFile="~/Views/Shared/Site.Master" **Inherits="MvcMusicStore.MyBasePage" %>**
	````

1. Include in **Browse** view a call to **MessageService,** that will display an image and a message retrieved by the service:

	````HTML(C#)
	<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="MvcMusicStore.Pages.MyBasePage" %>
	
	<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	    Browse Albums
	</asp:Content>
	
	<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	
	<div>
	<%= this.MessageService.Message %>
	<br />
	<img alt="<%: this.MessageService.Message %>"src="<%: this.MessageService.ImageUrl %>" />
	</div>
	...
	````

#### Task 2 - Including a Custom Dependency Resolver and a Custom View Page Activator ####

In the previous task, you injected a new dependency inside a view to perform a service call inside it. Now, you will start solving that dependence by implementing the new MVC 4 Dependency Injection Interfaces **IViewPageActivator** and **IDependencyResolver**.You will include in the solution an implementation of **IDependencyResolver** that will deal with service retrieval by using Unity. Then you will include another custom implementation of **IViewPageActivator** interface that will solve the creation of Views.

> **Note: MVC 4** implementation for Dependency Injection had simplified the interfaces for service registration. **IDependencyResolver** and **IViewPageActivator** are a part of the new MVC4 features for Dependency Injection.**- IDependencyResolver** interface replaces the previous IMvcServiceLocator. Implementers of IDependencyResolver must return an instance of the service or a service collection.

> **C#**

> **public interface IDependencyResolver {**

> **object GetService(Type serviceType);**

> **IEnumerable<object> GetServices(Type serviceType);**

> **}**

> **- IViewPageActivator** interface provides more fine-grained control over how view pages are instantiated via dependency injection. The classes that implement **IViewPageActivator** interface must create the instance of a view having context information.

> **C#**

> **public interface IViewPageActivator {**

> **object Create(ControllerContext controllerContext, Type type);**

> **}**

1. Copy the class **CustomViewPageActivator.cs** from **/Sources/Assets** to the**Factories** folder. This class implements the **IViewPageActivator** interface to hold the Unity Container.

	````C#-CustomViewPageActivator.cs
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Factories
	{
	    public class CustomViewPageActivator : IViewPageActivator
	    {
	        IUnityContainer container;
	
	        public CustomViewPageActivator(IUnityContainer container)
	        {
	            this.container = container;
	        }
	
	        public object Create(ControllerContext controllerContext, Type type)
	        {
	            return this.container.Resolve(type);
	        }
	    }
	}
	````

	**Note: CustomViewPageActivator** is responsive for managing the creation of a view by using a Unity container.

1. Add the class **UnityDependencyResolver.cs,** which is included in the folder **/Sources/Assets**, in the project folder **/Factories.**

	````C#-UnityDependencyResolver.cs
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Factories
	{
	    public class UnityDependencyResolver : IDependencyResolver
	    {
	        IUnityContainer container;
	        IDependencyResolver resolver;
	
	        public UnityDependencyResolver(IUnityContainer container, IDependencyResolver resolver)
	        {
	            this.container = container;
	            this.resolver = resolver;
	        }
	
	        public object GetService(Type serviceType)
	        {
	            try
	            {
	                return this.container.Resolve(serviceType);
	            }
	            catch
	            {
	                return this.resolver.GetService(serviceType);
	            }
	        }
	
	        public IEnumerable<object> GetServices(Type serviceType)
	        {
	            try
	            {
	                return this.container.ResolveAll(serviceType);
	            }
	            catch
	            {
	                return this.resolver.GetServices(serviceType);
	            }
	        }
	    }
	}
	````

	> **Note: UnityDependencyResolver** class is a custom DependencyResolver for Unity. When a service can't be found inside the Unity container, base resolver is invocated.  

In the following task both implementations will be registered, to let the model know where to locate the services and where to create the views.

 
#### Task 3 - Registering for Dependency Injection in Global.asax.cs Application_Start ####

In this task, you will put all the previous things together to make Dependency Injection Work.

Up to now you have the following elements:

- A**Browse** View that inherits from **MyBaseClass** and consumes MessageService.

- An intermediate **MyBaseClass** class that has dependency injection declared for the service interface.

- A **MessageService** service and its Interface **IMessageService**.

- A custom dependency resolver for Unity: **UnityDependencyResolver**, that deals with service retrieval.

- A View Page activator: **CustomViewPageActivator,** that creates the page.

 
To inject Browse View, you will now register the custom dependency resolver into the Unity container.

1. Open **Global.asax.cs** at project root.

1. Register an instance of **MessageService** into Unity container that will initialize the service:

	(Code Snippet - ASP.NET MVC Dependency Injection  - Ex02 Injecting a View - GlobalAsax Registration - Csharp)

	````C#
	protected void Application_Start()
	{
	    AreaRegistration.RegisterAllAreas();
	
	    RegisterGlobalFilters(GlobalFilters.Filters);
	    RegisterRoutes(RouteTable.Routes);
	            
	    var container = new UnityContainer();
	    container.RegisterType<IStoreService, StoreService>();
	    container.RegisterType<IController, StoreController>("Store");
	
	    var factory = new UnityControllerFactory(container);
	    ControllerBuilder.Current.SetControllerFactory(factory);
	
	    container.RegisterInstance<IMessageService>(new MessageService { 
	        Message = "You are welcome to our Web Camps Training Kit!",
	        ImageUrl = "/Content/Images/logo-webcamps.png"    
	    });
	    
	}
	````

1. Register **CustomViewPageActivator** as a view page activator into Unity container:

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex02 Injecting a View - GlobalAsax Registration 2 - Csharp)

	````C#
	protected void Application_Start()
	{
	...
	    container.RegisterInstance<IMessageService>(new MessageService { 
	        Message = "You are welcome to our Web Camps Training Kit!",
	        ImageUrl = "/Content/Images/logo-webcamps.png"    
	    });
	    container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));
	
	}
	````

1. Replace MVC 4 default dependency resolver with an instance of **UnityDependencyResolver**:

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex02 Injecting a View - GlobalAsax Registration 3 - Csharp)

	````C#
	protected void Application_Start()
	{
	    AreaRegistration.RegisterAllAreas();
	
	    RegisterGlobalFilters(GlobalFilters.Filters);
	    RegisterRoutes(RouteTable.Routes);
	            
	    var container = new UnityContainer();
	    container.RegisterType<IStoreService, StoreService>();
	    container.RegisterType<IController, StoreController>("Store");
	
	    var factory = new UnityControllerFactory(container);
	    ControllerBuilder.Current.SetControllerFactory(factory);
	
	    container.RegisterInstance<IMessageService>(new MessageService { 
	        Message = "You are welcome to our Web Camps Training Kit!",
	        ImageUrl = "/Content/Images/logo-webcamps.png"    
	    });
	    container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));
	
	    IDependencyResolver resolver = DependencyResolver.Current;
	
	    IDependencyResolver newResolver = new UnityDependencyResolver(container, resolver);
	
	    DependencyResolver.SetResolver(newResolver);
	}
	````

	> **Note:** ASP.NET MVC4 provides a default dependency resolver class. To work with custom dependency resolvers as the one we have created for unity, this resolver has to be replaced.

 
#### Task 4 - Running the Application ####

In this task, you will run the application to verify that the Store Browser consumes the service and shows the image and the message retrieved by it:

1. Press **F5** to run the application, where Home page will load.

1. Browse to **/Store** and enter to any of the genres that are shown below. In this example, we enter to "**Rock**":

 	![MVC Music Store - View Injection](./images/MVC_Music_Store_-_View_Injection.png?raw=true "MVC Music Store - View Injection")
 
	_MVC Music Store - View Injection_

1. Close the browser.

### Exercise 3: Injecting Action Filters ###

In a previous Lab about Custom Action Filters you have been working with filters customization and injection. In this exercise,you will learn how to inject filters with Dependency Injection by using Unity Application Block containers. To do that, you will add to the Music Store Solution a custom action filter that will trace site activity.

#### Task 1 - Including the Tracking Filter in the Solution ####

In this task, you will include in the Music Store a custom action filter for event tracing. As filters were treated in a previous Lab "Custom Action Filters", you will include the filter class from the Assets folder and then create a Filter Provider for Unity:

1. Open the begin solution at **/Source/Ex03 - Injecting Filters/Begin/MvcMusicStore.sln**.

1. Create the folder **/Filters** at project root.

1. Add the custom action filter **TraceActionFilter.cs** to the project in the folder **/Filters** that you can find it at**/Sources/Assets/TraceActionFilter.cs**.

	````C#-TraceActionFilter.cs
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	
	namespace MvcMusicStore.Filters
	{
	    public class TraceActionFilter : IActionFilter
	    {
	        public void OnActionExecuted(ActionExecutedContext filterContext)
	        {
	            filterContext.HttpContext.Trace.Write("OnActionExecuted");
	            filterContext.HttpContext.Trace.Write("Action " + filterContext.ActionDescriptor.ActionName);
	            filterContext.HttpContext.Trace.Write("Controller " + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
	        }
	
	        public void OnActionExecuting(ActionExecutingContext filterContext)
	        {
	            filterContext.HttpContext.Trace.Write("OnActionExecuting");
	            filterContext.HttpContext.Trace.Write("Action " + filterContext.ActionDescriptor.ActionName);
	            filterContext.HttpContext.Trace.Write("Controller " + filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
	        }
	    }
	}
	````

	**Note:** This custom action filter performs ASP.NET tracing. You can check "Global and Dynamic Action Filters" Lab for more reference.

1. Add the empty class **FilterProvider.cs** to the project in the folder **/Filters.**

1. Include **System.Web.Mvc** and **Microsoft.Practices.Unity** namespaces in **FilterProvider.cs**.

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex03 Injecting Action Filters - FilterProvider namespace - Csharp)

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Filters
	{
	    public class FilterProvider	    {
	    }
	}
	````

Make the class inherit from **IFilterProvider** Interface.
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Filters
	{
	    public class FilterProvider : **IFilterProvider**
	    {
	    }
	}
	````

1. Add a **IUnityContainer** property in **FilterProvider** class, and then create a class constructor to set the container:

	(Code Snippet - ASP.NET MVC Dependency Injection - Ex03 Injecting Action Filters - IUnityContainer - Csharp)

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Filters
	{
	    public class FilterProvider : IFilterProvider
	    {
	        IUnityContainer container;
	
	        public FilterProvider(IUnityContainer container)
	        {
	            this.container = container;
	        }
	
	    }
	}
	````

	> **Note:** The filter provider class constructor is not creating a **new** object inside. The container is passed as a parameter, and the dependency is solved by Unity.

1. Implement in **FilterProvider** class the method **GetFilters** from **IFilterProvider** interface:

	(Code Snippet - ASP.NET MVC Dependency Injection -  Ex03 Injecting Action Filters - GetFilters - Csharp)

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Filters
	{
	    public class FilterProvider : IFilterProvider
	    {
	        IUnityContainer container;
	
	        public FilterProvider(IUnityContainer container)
	        {
	            this.container = container;
	        }
	
	        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
	        {
	            foreach (IActionFilter actionFilter in container.ResolveAll<IActionFilter>())
	                yield return new Filter(actionFilter, FilterScope.First, null);
	        }
	    }
	}
	````

	**Note:** This implementation of **GetFilters** method is shorter than the one you have learned in "**Global and Dynamic Action Filters"** lab, where you ask if each controller or action was inside a list of accepted items.

 
#### Task 2 - Registering and Enabling the Filter ####

1. In this task, you will enable site tracking and then you will register the filter in **Global.asax.cs Application_Start** method to start tracing:

1. Open **Web.config** at project root and enable trace tracking at System.Web group:

	````XML
	  <system.web>
	    <trace enabled="true"/>
	    <compilation debug="true" targetFramework="4.0">
	````

1. Open **Global.asax.cs** at project root.

1. If you are using C#, add a reference to the Filters namespace:

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using System.Web.Routing;
	using Microsoft.Practices.Unity;
	using MvcMusicStore.Services;
	using MvcMusicStore.Factories;
	using MvcMusicStore.Controllers;
	using MvcMusicStore.Filters;
	
	namespace MvcMusicStore
	````

1. Select **Application_Start** method and register the filter in the Unity Container. You will have to register the filter provider and the action filter as well:

	(Code Snippet - ASP.NET MVC Dependency Injection -  Ex03 Injecting Action Filters - Global asax unity registration - CSharp)

	````C#
	protected void Application_Start()
	{
	    AreaRegistration.RegisterAllAreas();
	
	    RegisterGlobalFilters(GlobalFilters.Filters);
	    RegisterRoutes(RouteTable.Routes);
	            
	    var container = new UnityContainer();
	    container.RegisterType<IStoreService, StoreService>();
	    container.RegisterType<IController, StoreController>("Store");
	
	    var factory = new UnityControllerFactory(container);
	    ControllerBuilder.Current.SetControllerFactory(factory);
	
	    IDependencyResolver resolver = DependencyResolver.Current;
	
	    container.RegisterInstance<IMessageService>(new MessageService { 
	        Message = "You are welcome to our Web Camps Training Kit!",
	        ImageUrl = "/Content/Images/logo-webcamps.png"    
	    });
	    container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));
	
	    container.RegisterInstance<IFilterProvider>("FilterProvider", new FilterProvider(container));
	    container.RegisterInstance<IActionFilter>("LogActionFilter", new TraceActionFilter());
	
	    IDependencyResolver newResolver = new UnityDependencyResolver(container, resolver);
	
	    DependencyResolver.SetResolver(newResolver);
	}
	````
 
#### Task 3 - Running the Application ####

In this task, you will run the application and test that the custom action filter is tracing the activity:

1. Press **F5** to run the application.

1. Browse to **/Store** and choose '**Rock'** genre. You can browse to more genres if you want to.

 	![Music Store](./images/Music_Store.png?raw=true "Music Store")
 
	_Music Store_

1. Browse to **/Trace.axd** to see the Application Trace page, and then click the '**View Details'** link at the right column for **Store/**:

 	![Application Trace Log](./images/Application_Trace_Log.png?raw=true "Application Trace Log")
 
	_Application Trace Log_

 	![Application Trace - Request Details](./images/Application_Trace_Request_Details.png?raw=true "Application Trace - Request Details")
 
	_Application Trace - Request Details_

1. Close the browser.

## Summary ##

By completing this Hands-On Lab you have learned how to use Dependency Injection in MVC 3 by integrating Unity Application Block. To achieve that purpose you have used Dependency Injection inside controllers, views and action filters.

The following concepts were used:

- MVC 4 Dependency Injection new features

- Unity Application Block integration

- Dependency Injection in Controllers

- Dependency Injection in Views

- Dependency injection of Action Filters
