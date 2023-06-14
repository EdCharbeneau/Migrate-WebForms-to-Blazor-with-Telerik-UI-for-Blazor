---
layout: post
title: Assisted Migration
path: assisted-migration.md
---

## Assisted Migration

1. Use the **.NET Upgrade Assistant** to **Upgrade** the **eShopLegacyWebForms** project. 

* Start the upgrade process by right clicking on the **eShopLegacyWebForms** project. Then, choose **Upgrade** from the context menu.

![](3-upgrade.png)

* Select **Side-by-side** from the wizard in the main window.

![](upgrade-2.png)

* Then choose, **Existing project**. Then select **eShopTelerikBlazorServer** from the Existing project drop down.

![](upgrade-4.png)

* Next select **.NET 7**. At the time of writing this is the current version.

![](upgrade-6.png)

* Finally, click **Finish** to begin the upgrade.

![](upgrade-7.png)

![](upgrade-summary.png)

> Note: The summary page will attempt to initialize a YARP proxy and Web Adapters. These are typically used in MVC/Web API applications and are not applicable to this scenario. 

2. Remove the unused proxy settings. In this instance, the .NET Upgrade Assistant added 3 unused settings in Program.cs. These settings are generally used when migrating Web API controllers which this project doesn't have. 

> For some migration projects, starting both projects may be necessary. A side-by-side migration strategy will use both projects by forwarding web APIs from the source project to the target via proxy.  

* Find and remove the following lines from **Program.cs**

```csharp
// -approximate line numbers
// - 7, 8
builder.Services.AddSystemWebAdapters();
builder.Services.AddHttpForwarder();

// - 35
app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

```

* Remove the corresponding values in `appsettings.json`. The values will not cause any errors or warnings if they're not removed, but removing them will keep the file tidy.

```js
// approxmiate line numbers 8-10
,
  "AllowedHosts": "*",
  "ProxyTo": ""
```

3. Use the **.NET Upgrade Assistant** to migrate the models, classes, services from the **eShopLegacyWebForms** project. When using the Upgrade tool on a class, the target class will have its dependencies upgraded with it. For this example, the **Global.asax.cs** file will be used. Because it's the root class of the application, all of the app's dependencies will listed in the upgrade and chosen by default. 

> Using the Global.asax.cs may be not be productive for all scenarios. For example, in larger applications individual files or features might be better to target for simplicity. 

* Select the **Upgrade Class** link from the **Summary** screen, or the **Upgrade** menu.

![](upgrade-class.png)

* Choose the **Global** class from the **Classes** drop down and click **Upgrade**.

![](upgrade-class-2.png)

* The tool will list all dependencies of the selected class. For this example scenario, it will be all the entire application's contents. Because there is no upgrade path for **System.Web**, deselect these items from the list. The classes may be easier organize in **Flat view**, which is enabled on the upper right corner of the interface.

![](upgrade-class-3.png)

For simplicity this example will not include the migration steps for logging and **log4net**. Deselect the dependencies that use **log4net**. 

![](upgrade-class-4.png)

* Once the items have been selected/deselected, click the **Upgrade selection** button to continue.

![](upgrade-complete.png)

## Summary so far

<!-- write a summary of what we accomplished -->

* EF
* Services
* ...