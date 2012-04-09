<a name="HOLTop" />
#ASP.NET MVC 4 Dependency Injection#

---

<a name="Overview" />
## Overview ##

>**Note:** This Hands-on Lab assumes you have basic knowledge of **ASP.NET MVC** and **ASP.NET MVC 4 filters**. If you have not used **ASP.NET MVC 4 filters** before, we recommend you to go over **ASP.NET MVC Custom Action Filters** Hands-on Lab.

In Object Oriented Programming paradigm, objects work together in a collaboration model where there are contributors and consumers. Naturally, this communication model generates dependencies between objects and components, becoming difficult to manage when complexity increases.

![Class dependencies and model complexity](./images/Class_dependencies_and_model_complexity.png?raw=true "Class dependencies and model complexity")
 
_Class dependencies and model complexity_

You have probably heard about the Factory Pattern and the separation between the interface and the implementation using services, where the client objects are often responsible for service location.

Before introducing the Dependency Injection Pattern, you  will learn about the Inversion of Control (IoC) pattern. With **Inversion of Control (IoC)**, consumer objects do not create the other objects on which they rely. Those objects come from an external source.

### The Dependency Injection (DI) Design Pattern ###

The Dependency injection (DI) design pattern separates the component behavior from the resolution of dependencies without object intervention.

This pattern is a particular implementation of Inversion of Control, where the consumer object receives the dependencies inside constructor properties or arguments.

DI requires a framework component behind to deal with the class constructors.

![Overview - Dependency Injection diagram](./images/Overview_Dependency_Injection_diagram.png?raw=true "Overview - Dependency Injection diagram")
 
_Overview - Dependency Injection diagram_

The advantages of using Dependency Injection pattern and Inversion of Control are the following:

- Reduces class coupling

- Increases code reusing

- Improves code maintainability

- Improves application testing

 
>**Note:** Dependency Injection is sometimes compared with Abstract Factory Design Pattern, but there is a slight difference between both approaches. DI has a Framework working behind to solve dependencies by calling the factories and the registered services.

Now that you understand the Dependency Injection Pattern, you will learn throughout this lab how to apply it in ASP.NET MVC 4. You will start using Dependency Injection in the **Controllers** to include a database access service. Next, you will apply Dependency Injection to the **Views** to consume a service and show information. Finally, you will extend the DI to MVC 4 Filters, injecting a custom action filter in the solution.

In this Hands-on Lab, you will learn how to:

- Integrate MVC 4 with Unity Application Block for Dependency Injection

- Integrate MVC 4 with MEF 2.0

- Use Dependency Injection inside an MVC Controller

- Use Dependency Injection inside an MVC View

- Use Dependency Injection inside an MVC Action Filter

 
>**Note:** This Lab is using Unity Application Block and MEF 2.0 as the frameworks for dependency resolution, but it is possible to adapt any Dependency Injection Framework to work with MVC 4.

<a name="SystemRequirements" />
### System Requirements ###

You must have the following items to complete this lab:

- Visual Studio 11 Express Beta for Web

<a name="Setup" />
### Setup ###

_**Installing Code Snippets**_

For convenience, much of the code you will be managing along this lab is available as Visual Studio code snippets. To install the code snippets run **.\Source\Assets\CodeSnippets.vsi** file.

_**Installing Web Platform Installer**_

This section assumes that you don't have some or all the system requirements installed. In case you do, you can simply skip this section.

Microsoft Web Platform Installer (WebPI) is a tool that manages the installation of the prerequisites for this Lab.

>**Note:** As well as the Microsoft Web Platform, WebPI can also install many of the open source applications that are available like Umbraco, Kentico, DotNetNuke and many more.  These are very useful for providing a foundation from which to customize an application to your requirements, dramatically cutting down your development time.

Please follow these steps to download and install Microsoft Visual Studio 11 Express Beta for Web:

1. Install **Visual Studio 11 Express Beta for Web**. To do this, navigate to [http://www.microsoft.com/web/gallery/install.aspx?appid=VWD11_BETA&prerelease=true](http://www.microsoft.com/web/gallery/install.aspx?appid=VWD11_BETA&prerelease=true) using a web browser. 

	![Web Platform Installer 4.0 window](./images/Microsoft-Web-Platform-Installer-4.png?raw=true "Web Platform Installer 4.0 download")

	_Web Platform Installer 4.0 download_

1. The Web Platform Installer launches and shows Visual Studio 11 Express Beta for Web Installation. Click on **Install**.

 	![Visual Studio 11 Express Beta for Web Installer window](./images/Microsoft-VS-11-Install.png?raw=true "Visual Studio 11 Express Beta for Web Installer window")
 
	_Visual Studio 11 Express Beta for Web Installer window_

1. The **Web Platform Installer** displays the list of software to be installed. Accept by clicking **I Accept**.

 	![Web Platform Installer window](./images/Microsoft-Web-Platform-Installer-Prerequisites.png?raw=true "Web Platform Installer window")
 
	_Web Platform Installer window_

1. The appropriate components will be downloaded and installed.

 	![Web Platform Installation - Download progress](./images/Web-Platform-Installation-Download-progress.png?raw=true "Web Platform Installation - Download progress")
 
	_Web Platform Installation - Download progress_

1. The **Web Platform Installer** will resume downloading and installing the products. When this process is finished, the Installer will show the list of all the software installed. Click **Finish**.

 	![Web Platform Installer](./images/Web-Platform-Installer.png?raw=true "Web Platform Installer")
 
	_Web Platform Installer_

---

<a name="Exercises" />
## Exercises ##

This Hands-On Lab is comprised by the following exercises:

1. [Exercise 1: Injecting a Controller](#Exercise1)

1. [Exercise 2: Injecting a View](#Exercise2)

1. [Exercise 3: Injecting Filters](#Exercise3)

 
Estimated time to complete this lab: **30 minutes**.

> **Note:** Each exercise is accompanied by an **End** folder containing the resulting solution you should obtain after completing the exercises. You can use this solution as a guide if you need additional help working through the exercises.

<a name="Exercise1" />
### Exercise 1: Injecting a Controller ###

In this exercise, you will learn how to use Dependency Injection in MVC Controllers by integrating Unity Application Block. For that reason, you will include services into your MVC Music Store controllers to separate the logic from the data access. The services will create a new dependence in the controller constructor, which will be resolved using Dependency Injection with the help of **Unity** Application Block.

This approach will show you how to generate less coupled applications, which are more flexible and easier to maintain and test. You will also learn how to integrate MVC with Unity.

#### About StoreManager Service ####

The MVC Music Store provided in the begin solution now includes a service that manages the Store Controller data named **StoreService**. Below you will find the Store Service implementation. Note that all the methods return Model entities.

````C#
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

 **StoreController** from the begin solution now consumes **StoreService**. All the data references were removed from **StoreController**, and now possible to modify the current data access provider without changing any method that consumes **StoreService**.

You will find below that the **StoreController** implementation has a dependency with  **StoreService** inside the class constructor.

> **Note:** The dependency introduced in this exercise is related to **Inversion of Control** (IoC).
>
> The **StoreController** class constructor receives an **IStoreService** type parameter, which is essential to perform service calls from inside the class. However, **StoreController** does not implement the default constructor (with no parameters) that any controller must have to work with MVC.
>
> To resolve the dependency, the controller has to be created by an abstract factory (a class that returns any object of the specified type).

````C#
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

>**Note:** You will get an error when the class tries to create the StoreController without sending the service object, as there is no parameterless constructor declared.
Throughout this lab you will learn how to deal with this problem using Dependency Injection with Unity and MEF.

<a name="Ex1Task1" />
#### Task 1 - Running the Application ####

In this task, you will run the Begin application, which includes the service into the Store Controller that separates the data access from the application logic.

After browsing to the store you will receive an exception, as the controller service is not passed as a parameter by default:

1. Open the begin solution **MvcMusicStore.sln** located in **Source\Ex01-Injecting Controller\Begin**.

1.	Follow these steps to install the **NuGet** package dependencies.

	1.	Open the **NuGet** **Package Manager Console**. To do this, select **Tools | Library Package Manager | Package Manager Console**.

	1.	In the **Package Manager Console,** type **Install-Package NuGetPowerTools**.

	1.	After installing the package, type **Enable-PackageRestore**.

	1.	Build the solution. The **NuGet** dependencies will be downloaded and installed automatically.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Press **F5** to run the application.

1. Browse to **/Store** to load Store Controller. You will get the error message "**No parameterless constructor defined for this object**":

 	![Error while running MVC Begin Application](./images/Error_while_running_MVC_Begin_Application.png?raw=true "Error while running MVC Begin Application")
 
	_Error while running MVC Begin Application_

1. Close the browser.

In the following steps you will work on the Music Store Solution to inject the dependency this controller needs.

<a name="Ex1Task2" />
#### Task 2 - Including Unity into MvcMusicStore Solution ####

In this task, you will include Unity Application Block 2.0 in the solution.

>**Note:** The Unity Application Block (Unity) is a lightweight, extensible dependency injection container with optional support for instance and type interception. It is a general-purpose container for use in any type of .NET application. It provides all the common features found in dependency injection mechanisms including: object creation, abstraction of requirements by specifying dependencies at runtime and flexibility, by deferring the component configuration to the container.

You can read more about Unity 2.0 at [msdn](http://msdn.microsoft.com/en-us/library/ff663144.aspx).

1. In the **MvcMusicStore** project, add a reference to **Microsoft.Practices.Unity.dll**, which is included in the **Source\Assets\Unity 2.0\** folder of this lab.

<a name="Ex1Task3" />
#### Task 3 - Adding a Unity Controller Factory ####

In this task, you will add a custom controller factory for **Unity**. This class implements the **IControllerFactory** interface, extending **CreateController** and **ReleaseController** methods to work with Unity. This factory will create the instances of the controllers that work with Dependency Injection.

>**Note:** A controller factory is an implementation of the _IControllerFactory_ interface, which is responsible for locating and for creating an instance of the controller type. The following implementation of **CreateController** finds the controller by its name in the Unity container and returns an instance if it is found. Otherwise, it delegates the creation of the controller to an inner factory. One of the advantages of this logic is that controllers can be registered by name. You can find **IControllerFactory** interface reference at [msdn](http://msdn.microsoft.com/en-us/library/system.web.mvc.icontrollerfactory.aspx).

1. In the **MvcMusicStore** project, create a new folder named **Factories**, and add the **UnityControllerFactory** class, which is included in the **Source\Assets\C#** folder of this lab.

	````C# 
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

	>**Note:** This factory class can be reused in any project that uses Dependency Injection for Controllers.

<a name="Ex1Task4" />
####Task 4 - Registering Unity in Global.asax.cs Application_Start ####

In this task, you will register the Unity library into the method **Application_Start** located in **Global.asax.cs**.

1. Open **Global.asax.cs**.

1. Include the following namespaces **Microsoft.Practices.Unity**, **MvcMusicStore.Services**, **MvcMusicStore.Factories** and **MvcMusicStore.Controllers**.

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex01 Injecting Controllers Global using_)

	<!-- mark:7-11 -->
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

1. Create a new Unity Container in **Global.asax.cs** **Application_Start**, and register the Store Service and the Store Controller.

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex01 Injecting Controllers Unity Container_)

	<!-- mark:6-9 -->
	````C#
	...
	protected void Application_Start()
	{
		...
	            
		var container = new UnityContainer();
		container.RegisterType<IStoreService, StoreService>();
		container.RegisterType<IController, StoreController>("Store");	
	}
	...
	````

1. Register the **UnityControllerFactory** type as the factory that will be used for creating controller instances:

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex01 Injecting Controllers Global application start_)

	<!-- mark:10-12 -->
	````C#
	...
	protected void Application_Start()
	{
		...
		           
		var container = new UnityContainer();
		container.RegisterType<IStoreService, StoreService>();
		container.RegisterType<IController, StoreController>("Store");

		var factory = new UnityControllerFactory(container);
		ControllerBuilder.Current.SetControllerFactory(factory);
	}
	...
	````

	>**Note:** **ControllerBuilder** is an MVC class responsible for dynamically building a controller. You can read more about ControllerBuilder at [msdn](http://msdn.microsoft.com/en-us/library/system.web.mvc.controllerbuilder.aspx).

<a name="Ex1Task5" />
####Task 5 - Running the Application ####

In this task, you will run the application to verify that the Store can now be loaded after including Unity.

1. Press **F5** to run the application.

1. Browse to **/Store**. This will invoke **StoreController**, which is now created by using **UnityControllerFactory**.

 	![MVC Music Store](./images/MVC_Music_Store.png?raw=true "MVC Music Store")
 
	_MVC Music Store_

1. Close the browser.

In the following exercises you will learn how to extend the Dependency Injection scope to use it inside MVC Views and Action Filters.

<a name="Exercise2" />
###Exercise 2: Injecting a View ###

In this exercise, you will learn how to use Dependency Injection in a view with the new features of MVC 4 for Unity integration. In order to do that, you will call a custom service inside the Store Browse View, which will show a message and an image below.

Then, you will integrate the project with Unity Application Block and create a custom dependency resolver to inject the dependencies.

<a name="Ex2Task1" />
#### Task 1 - Creating a View that Consumes a Service ####

In this task, you will create a view that performs a service call to generate a new dependency. The service consists in a simple messaging service included in this solution.

1. Open the begin solution **MvcMusicStore.sln** in the **Source\Ex02-Injecting View\Begin** folder.

1.	Follow these steps to install the **NuGet** package dependencies.

	1.	Open **NuGet** **Package Manager Console**. To do this, select **Tools | Library Package Manager | Package Manager Console**.

	1.	In the **Package Manager Console,** type **Install-Package NuGetPowerTools**.

	1.	After installing the package, type **Enable-PackageRestore**.

	1.	Build the solution. The **NuGet** dependencies will be downloaded and installed automatically.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Include the **MessageService.cs** and the **IMessageService.cs** classes located in the **Source\Assets\C#** folder in **/Services**.

	>**Note:** The **IMessageService** interface defines two properties implemented by the **MessageService** class. These properties -**Message** and **ImageUrl**- store the message and the URL of the image to be displayed.

1. Create the folder **/Pages** in the project's root folder, and then add the existing class **MyBasePage.cs** from **Source\Assets\C#**. The base page you will inherit from has the following structure.

	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using Microsoft.Practices.Unity;
	using MvcMusicStore.Services;
	
	namespace MvcMusicStore.Pages
	{
	    public class MyBasePage : System.Web.Mvc.WebViewPage<MvcMusicStore.ViewModels.StoreBrowseViewModel>
	    {
			 [Dependency]
			 public IMessageService MessageService { get; set; }
			  
			 public override void Execute()
			 {
			 }
	    }
	}
	````

	>**Note:** Due to a technical issue in the **ASP.NET** engine, the **dependency** at **IMessageService** interface cannot be included in the respective **View Model** Class.
	>
	> For that reason, **MyBasePage** intercepts the relationship between the View and the View-Model, and the dependency injection at MessageService can now be inherited by the View.

1. Open **Browse.cshtml** view from **/Views/Store** project folder, and make it inherit from **MyBasePage.cs**.

	````C#
	@inherits MvcMusicStore.Pages.MyBasePage
	````

1. In the **Browse** view, add a call to **MessageService** to display an image and a message retrieved by the service.

	<!-- mark:7-12 -->
	````HTML(C#)
	@inherits MvcMusicStore.Pages.MyBasePage
	
	@{
		Viewbag.Title = "Browse Albums";
	}

	<div>
		@this.MessageService.Message
		<br />
		<img alt="@this.MessageService.Message" src="@this.MessageService.ImageUrl" />
	</div>
	...
	````

<a name="Ex2Task2" />
#### Task 2 - Including a Custom Dependency Resolver and a Custom View Page Activator ####

In the previous task, you injected a new dependency inside a view to perform a service call inside it. Now, you will resolve that dependency by implementing the MVC Dependency Injection interfaces **IViewPageActivator** and **IDependencyResolver**. You will include in the solution an implementation of **IDependencyResolver** that will deal with the service retrieval by using Unity. Then, you will include another custom implementation of **IViewPageActivator** interface that will solve the creation of the views.

> **Note:** Since MVC 3, the implementation for Dependency Injection had simplified the interfaces to register services. **IDependencyResolver** and **IViewPageActivator** are part of MVC 3 features for Dependency Injection.
>
>**- IDependencyResolver** interface replaces the previous IMvcServiceLocator. Implementers of IDependencyResolver must return an instance of the service or a service collection.
>
>````C#
>	public interface IDependencyResolver {
>		object GetService(Type serviceType);
>		IEnumerable<object> GetServices(Type serviceType);
>	}
>````
>
> **- IViewPageActivator** interface provides more fine-grained control over how view pages are instantiated via dependency injection. The classes that implement **IViewPageActivator** interface can create view instances using context information.
>
>````C#
>	public interface IViewPageActivator {
>		object Create(ControllerContext controllerContext, Type type);
>	}
>````

1. Copy **CustomViewPageActivator.cs** from **/Sources/Assets/C#** to **Factories** folder. To do that, right-click  the **/Factories** folder, select **Add | Existing Item** and then select the recently copied file. This class implements the **IViewPageActivator** interface to hold the Unity Container.
 
	````C#
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

	>**Note:** **CustomViewPageActivator** is responsible for managing the creation of a view by using a Unity container.

1. Copy **UnityDependencyResolver.cs** from **/Sources/Assets/C#** to  **/Factories** folder and add the class to the project. To do that, right-click  the **/Factories** folder, select **Add | Existing Item** and then select the recently copied file.

	````C#
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

	>**Note:** **UnityDependencyResolver** class is a custom DependencyResolver for Unity. When a service cannot be found inside the Unity container, the base resolver is invocated.  

	
In the following task both implementations will be registered to let the model know the location of the services and the views.

<a name="Ex2Task3" />
#### Task 3 - Registering for Dependency Injection in Global.asax.cs Application_Start ####

In this task, you will put all the previous things together to make Dependency Injection work.

Up to now your solution has the following elements:

- A **Browse** View that inherits from **MyBaseClass** and consumes **MessageService**.

- An intermediate class -**MyBaseClass**- that has dependency injection declared for the service interface.

- A service - **MessageService** - and its interface **IMessageService**.

- A custom dependency resolver for Unity - **UnityDependencyResolver** - that deals with service retrieval.

- A View Page activator - **CustomViewPageActivator ** that creates the page.

 
To inject Browse View, you will now register the custom dependency resolver in the Unity container.

1. Open **Global.asax.cs**.

1. Register an instance of **MessageService** into the Unity container to initialize the service:

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex02 Injecting a View GlobalAsax Registration_)

	<!-- mark:12-16 -->
	````C#
	protected void Application_Start()
	{
		...

		var container = new UnityContainer();
		container.RegisterType<IStoreService, StoreService>();
		container.RegisterType<IController, StoreController>("Store");

		var factory = new UnityControllerFactory(container);
		ControllerBuilder.Current.SetControllerFactory(factory);

		container.RegisterInstance<IMessageService>(new MessageService { 
			Message = "You are welcome to our Web Camps Training Kit!",
			ImageUrl = "/Content/Images/webcamps.png"    
		});

		...
	}
	````

1. Register **CustomViewPageActivator** as a View Page activator into the Unity container:

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex02 Injecting a View GlobalAsax Registration 2_)

	<!-- mark:17-18 -->
	````C#
	protected void Application_Start()
	{
		...

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

		...
	}
	````

1. Replace MVC 4 default dependency resolver with an instance of **UnityDependencyResolver**:

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex02 Injecting a View GlobalAsax Registration 3_)

	<!-- mark:19-24 -->
	````C#
	protected void Application_Start()
	{
		...

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

		IDependencyResolver resolver = DependencyResolver.Current;

		IDependencyResolver newResolver = new UnityDependencyResolver(container, resolver);

		DependencyResolver.SetResolver(newResolver);
	
		...
	}
	````

	>**Note:** ASP.NET MVC provides a default dependency resolver class. To work with custom dependency resolvers as the one we have created for unity, this resolver has to be replaced.

<a name="Ex2Task4" />
#### Task 4 - Running the Application ####

In this task, you will run the application to verify that the Store Browser consumes the service and shows the image and the message retrieved:

1. Press **F5** to run the application.

1. Browse to **/Store** and enter to any of the genres that are shown below. In this example, we are entering to "**Rock**":

 	![MVC Music Store - View Injection](./images/Music_Store.png?raw=true "MVC Music Store - View Injection")
 
	_MVC Music Store - View Injection_

1. Close the browser.

<a name="Exercise3" />
### Exercise 3: Injecting Action Filters ###

In the previous Hands-On lab **Custom Action Filters** you have worked with filters customization and injection. In this exercise, you will learn how to inject filters with Dependency Injection by using the Unity Application Block container. To do that, you will add to the Music Store solution a custom action filter that will trace the activity of the site.

<a name="Ex3Task1" />
#### Task 1 - Including the Tracking Filter in the Solution ####

In this task, you will include in the Music Store a custom action filter to trace events. As custom action filter concepts are already treated in the previous Lab "Custom Action Filters", you will just include the filter class from the Assets folder of this lab, and then create a Filter Provider for Unity:

1. Open the begin solution at **/Source/Ex03 - Injecting Filters/Begin/MvcMusicStore.sln**.

1. Follow these steps to install the **NuGet** package dependencies.

	1.	Open the **NuGet** **Package Manager Console**. To do this, select **Tools | Library Package Manager | Package Manager Console**.

	1.	In the **Package Manager Console,** type **Install-Package NuGetPowerTools**.

	1.	After installing the package, type **Enable-PackageRestore**.

	1.	Build the solution. The **NuGet** dependencies will be downloaded and installed automatically.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Copy **TraceActionFilter.cs** from **/Sources/Assets/C#** to **/Filters** project folder. Then, in the Solution Explorer, add the file to **/Filters** folder.

	````C#
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

	>**Note:** This custom action filter performs ASP.NET tracing. You can check "MVC 4 local and Dynamic Action Filters" Lab for more reference.

1. Add the empty class **FilterProvider.cs** to the project in the folder **/Filters.**

1. Add the **System.Web.Mvc** and **Microsoft.Practices.Unity** namespaces in **FilterProvider.cs**.

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex03 Injecting Action Filters FilterProvider namespace_)
	
	<!-- mark:5-7 -->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Microsoft.Practices.Unity;
	
	namespace MvcMusicStore.Filters
	{
	    public class FilterProvider	{
	    }
	}
	````

1. Make the class inherit from **IFilterProvider** Interface.

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
	    }
	}
	````

1. Add a **IUnityContainer** property in the **FilterProvider** class, and then create a class constructor to assign the container.

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex03 Injecting Action Filters IUnityContainer_)
	
	<!-- mark:12-17 -->
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

	>**Note:** The filter provider class constructor is not creating a **new** object inside. The container is passed as a parameter, and the dependency is solved by Unity.

1.In the **FilterProvider** class, implement the method **GetFilters** from **IFilterProvider** interface:

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex03 Injecting Action Filters GetFilters_)

	<!-- mark:19-23 -->
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

<a name="Ex3Task2" />
#### Task 2 - Registering and Enabling the Filter ####

1. In this task, you will enable site tracking. To do that, you will register the filter in **Global.asax.cs Application_Start** method to start tracing:

1. Open **Web.config** located in the project root and enable trace tracking at System.Web group.

	````XML
	  <system.web>
	    <trace enabled="true"/>
	    <compilation debug="true" targetFramework="4.5">
	````

1. Open **Global.asax.cs** at project root.

1. Add a reference to the Filters namespace.

	<!-- mark:11 -->
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

1. Select the **Application_Start** method and register the filter in the Unity Container. You will have to register the filter provider as well as the action filter.

	(Code Snippet - _ASP.NET MVC 4 Dependency Injection - Ex03 Injecting Action Filters Global asax unity registration_)

	<!-- mark:5-7 -->
	````C#
	protected void Application_Start()
	{
		container.RegisterType<IViewPageActivator, CustomViewPageActivator>(new InjectionConstructor(container));

	    container.RegisterInstance<IFilterProvider>("FilterProvider", new FilterProvider(container));
	    container.RegisterInstance<IActionFilter>("LogActionFilter", new TraceActionFilter());

        IDependencyResolver resolver = DependencyResolver.Current;
	}
	````

<a name="Ex3Task3" />
#### Task 3 - Running the Application ####

In this task, you will run the application and test that the custom action filter is tracing the activity:

1. Press **F5** to run the application.

1. Browse to **/Store** and select **Rock** genre. You can browse to more genres if you want to.

 	![Music Store](./images/Music_Store.png?raw=true "Music Store")
 
	_Music Store_

1. Browse to **/Trace.axd** to see the Application Trace page, and then click **View Details**.

 	![Application Trace Log](./images/Application_Trace_Log.png?raw=true "Application Trace Log")
 
	_Application Trace Log_

 	![Application Trace - Request Details](./images/Application_Trace_Request_Details.png?raw=true "Application Trace - Request Details")
 
	_Application Trace - Request Details_

1. Close the browser.

<a name="Exercise4" />
### Exercise 4: Injecting a Controller using MEF 2.0 ###

In this exercise, you will learn how to inject dependencies in a controller but using a different container. 

<a name="Ex4Task1" />
#### Task 1 - Running the Application ####

In this task, you will run the Begin application, which includes the service into the Store Controller that separates the data access from the application logic.

After browsing to the store you will receive an exception, as the controller service is not passed as a parameter by default:

1. Open the begin solution **MvcMusicStore.sln** at **Source\Ex04-Injecting Controller using MEF 2.0\Begin**.

1.	Follow these steps to install the **NuGet** package dependencies.

	1.	Open the **NuGet** **Package Manager Console**. To do this, select **Tools | Library Package Manager | Package Manager Console**.

	1.	In the **Package Manager Console,** type **Install-Package NuGetPowerTools**.

	1.	After installing the package, type **Enable-PackageRestore**.

	1.	Build the solution. The **NuGet** dependencies will be downloaded and installed automatically.

	>**Note:** One of the advantages of using NuGet is that you don't have to ship all the libraries in your project, reducing the project size. With NuGet Power Tools, by specifying the package versions in the Packages.config file, you will be able to download all the required libraries the first time you run the project. This is why you will have to run these steps after you open an existing solution from this lab.
	
	>For more information, see this article: <http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages>.

1. Press **F5** to run the application.

1. Browse to **/Store** to load Store Controller. You will get the error message "**No parameterless constructor defined for this object**":

 	![Error while running MVC Begin Application](./images/Error_while_running_MVC_Begin_Application.png?raw=true "Error while running MVC Begin Application")
 
	_Error while running MVC Begin Application_

1. Close the browser.

In the following task you will work on the Music Store Solution to inject the required dependencies.

<a name="Ex4Task2" />
#### Task 2 - Including MEF 2.0 into MvcMusicStore Solution ####

In this task, you will install MEF 2.0 in your solution.

You can read more about MEF 2.0 at [codeplex](http://mef.codeplex.com/).

1. In the Package Manager Console execute the following command.

	````PMC
	install-package Microsoft.Mef.MvcCompositionProvider -pre
	````

	>**Note:** After installing MEF 2.0 you will notice a new folder named **Parts** with a file inside named **Part1.cs**

1. Delete the **Part1.cs** file.

<a name="Ex4Task3" />
#### Task 3 - Adding the service to the constructor ####

In this task, you will add the service in the controller.

1. Move **IStoreService** and **StoreService**, located in the **Services** folder, to the **Parts** folder.

1. In **IStoreService** and **StoreService** files, change the namespace **MvcMusicStore.Services** to **MvcMusicStore.Parts**.

	<!-- mark:1 -->
	````C#
	namespace MvcMusicStore.Parts;
	````

1. In **StoreController.cs**, change **MvcMusicStore.Services** to **MvcMusicStore.Parts**.

	<!-- mark:8 -->
	````C#
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using MvcMusicStore.ViewModels;
	using MvcMusicStore.Models;
	using MvcMusicStore.Parts;
	````

<a name="Ex4Task4" />
#### Task 4 - Running the Application ####

In this task, you will run the application to verify that the Store can now be loaded after including MEF.

1. Press **F5** to run the application.

1. Browse to **/Store**.

 	![MVC Music Store](./images/MVC_Music_Store.png?raw=true "MVC Music Store")
 
	_MVC Music Store_

1. Close the browser.

---

<a name="Summary" />
## Summary ##

By completing this Hands-On Lab you have learned how to use Dependency Injection in MVC 4 by integrating Unity Application Block and MEF 2.0. To achieve that, you have used Dependency Injection inside controllers, views and action filters.

The following concepts were covered:

- MVC 4 Dependency Injection features

- Unity Application Block and MEF 2.0 integration

- Dependency Injection in Controllers

- Dependency Injection in Views

- Dependency injection of Action Filters
