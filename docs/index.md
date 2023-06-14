---
title: Introduction
date: 2024-06-14
categories: prerequisite
tags: intro
---
## Prerequisites 

<!-- ??? 
```
		<UseRazorSourceGenerator>true</UseRazorSourceGenerator>
		<Nullable>enable</Nullable>
```		
-->
To complete the workshop be sure to install all of the following prerequisite items. All of the items below have free, or free trial options. If you have existing paid licenses of the Visual Studio 2022 or Telerik UI for Blazor please use those and do not reinstall the free versions.

* [Visual Studio 2022 [Any Edition]](https://visualstudio.microsoft.com/downloads/)
* [.NET 7.0.5 or later](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
* Telerik UI for Blazor [Free Trial or Licensed](https://www.telerik.com/try/ui-for-blazor)
* [.NET Upgrade Assistant for VS 2022 (Preview)](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.upgradeassistant)
* [eShopModernizing GitHub Repository](https://github.com/dotnet-architecture/eShopModernizing)

## Getting started

1. Clone the [eShop Repository](https://github.com/dotnet-architecture/eShopModernizing) which contains the sample Web Forms application. 

Throughout this workshop we will be migrating the eShop sample application provided by Microsoft via GitHub. If you have not already created a clone of the repository, create one now. This repository contains multiple eShop solutions. For this workshop we will be using the **eShopLegacyWebForms** solution.

2. Open the eShopLegacyWebForms solution and install any missing frameworks. This step may vary from system to system.

After cloning the application, navigate to the solution in Visual Studio. You will see multiple solution files in the repository, choose **eShopLegacyWebForms**. Be aware of any notifications from Visual Studio that might inform you of additional dependencies that will need to be installed. Look for a yellow banner with an **Install** link in the **Solution Explorer**. Click install, follow the prompts and complete the installation as needed.

![](_img/1-installing-frameworks-vs.png) Reboot may be necessary

3. Start the eShopLegacyWebForms application. From Visual Studio click **Start** or press **F5**. The sample application is a simple store management page with view, create, edit, and delete functions.

> Troubleshooting tip: If the application does not run on the first try, Start the application a second time. The first time building the project occasionally hits a race condition where resources are not yet ready for the application to run. Running the app a second time typically works.

![](2-eshop-start.png)

4. After verifying the application starts properly, close the browser or stop the application from Visual Studio.

Take a moment to review the structure of the application and become familiar with its parts.

* **App_Start**, this folder contains the startup routines for the application.
* **Catalog**, this folder contains the Create, Delete, Details, and Edit features for the application.
* **Models**, this folder contains various data models for the application and its database. 
* **Modules**, this folder contains dependency injection (DI) configurations for AutoFac.
* **Services**, this folder contains data services used in the application.
* **ViewModel**, this folder contains a class used as a data projection for the application's main view.

5. Create a branch named `migration`. This branch will contain the migrated application and track changes to the application.

![](create-branch.png)

## Creating a new project

Because of the number of differences between .NET Framework and ASP.NET Core, the app will be migrated to a brand new solution. In addition, there is no direct migration path for Web Forms views in ASP.NET Core and the views will need to be completely rewritten. The Telerik UI for Blazor and its templates, scaffolding tools, and components will reduce the amount of coding needed to write new views.

> The Telerik Extensions for Visual Studio are added to Visual Studio when installing Telerik UI for Blazor. If the extension isn't installed, it can be added from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=TelerikInc.ProgressTelerikBlazorVSExtensions), or the Telerik UI for Blazor installer. Important: Telerik UI for Blazor is required to make use of the extension.

1. Create a new Telerik UI for Blazor Server application. This application will be the migration target. To create a Telerik UI for Blazor application in the solution:

* Right click on the root item in the solution, **Solution, eShopLegacyWebForms**.
* Select the **Telerik C# Blazor Application** template.

![](add-telerik-project.png)

* Name the project **eShopTelerikBlazorServer**.

![](add-telerik-project-1.png)

* Select a Telerik theme. The **Default-Ocean-Blue-A11y** is a recommended starting point because the colors have been tested for optimal accessibility. 

![](add-telerik-project-3.png)

* For the workshop the default project settings will be used. When using Telerik UI for Blazor on projects that require localization, the option can be enabled here and localization resources will be generated. Click finish to generate the new project.

![](add-telerik-project-4.png)

2. Set the startup project to the Blazor Server project,  **eShopTelerikBlazorServer**. Choose **Configure Startup Projects** from the **Project** menu.

![](startup-project-1.png)

![](startup-project-2.png)

* Select the **eShopTelerikBlazorServer** project from the **Single startup project** option. This menu can be revisited at any time to switch startup projects, or enable multiple startup projects.

3. Start the **eShopTelerikBlazorServer** project by clicking **Start** or pressing **F5**.

![](telerik-project.png)

4. Explore the new application to see an example of Telerik UI for Blazor's components and capabilities.

* `/` the root page demonstrates the window component and notification components. The code for this view is located in `Index.razor`.
* `/grid` the grid page displays a sample Telerik Grid component with Create, Read, Update and Delete operations.
* `/chart` the chart page shows one of the many charts available in Telerik UI for Blazor.
* `/form` the form page uses the Telerik Form component and its automatic form generation features.
