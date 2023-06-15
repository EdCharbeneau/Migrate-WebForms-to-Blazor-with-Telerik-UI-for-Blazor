---
title: 3. Manual Migration
nav_order: 3
---

# Manual migration

The **.NET Upgrade Assistant** has upgraded the project as much has possible using automation and it is now time to manually work on the project. In its current state, the project is not able to run. Before the application will start, some of the code needs to be manually rewritten. The following are highlights of what needs to be completed before the migration is complete.

* There is no **Global.asax.cs** in ASP.NET Core. This file must be manually migrated to **Program.cs**
* Configuration has breaking changes between ASP.NET Framework and ASP.NET Core.
* Dependency injection (DI) is included in ASP.NET Core. 3rd party DI can be used, however manual changes are required to do so.
* Web Forms views **.aspx** must be completely reconstructed as Razor Components **.razor**.

## Migrating Global.asax.cs

ASP.NET Core applications are standardized to follow the same conventions as the rest of the .NET ecosystem. The startup routine of an ASP.NET Core application uses a Program.cs file same as .NET Console, .NET MAUI, and many other application models. The **Program.cs** file is the next best equivalent to the **Global.asax.cs** file.

1. Examine the **Global.asax.cs** in **eShopLegacyWebForms**. Lines 29 & 30, were part of System.Web and not compatible with ASP.NET Core. RouteConfig is now handled by Razor Components through route directives. In addition, BundleConfig is no longer used and viable replacements such as WebPack can be used as needed. In this example, the Telerik UI for Blazor will introduce enough functionality out-of-the-box to replace many of the resources (.js and .css) typically bundled by WebPack. 

    Next is **ConfigureContainer**. In Web Forms this method would configure the Dependency Injection (DI) container using AutoFac. In ASP.NET Core, DI is included as **Microsoft.Extensions.DependencyInjection** through the **IServiceCollection** interface. Because the legacy application used AutoFac it will be easier to migrate the existing configuration to a updated version of AutoFac that supports ASP.NET Core's **IServiceCollection**.

    Last is **ConfigDataBase**. In Web Forms this method setup the EntityFramework configuration and database initialization methods. Included is a mock data setting used to run the application with an in memory database instead of SQL server.

    ```csharp
    protected void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        RouteConfig.RegisterRoutes(RouteTable.Routes); // 29
        BundleConfig.RegisterBundles(BundleTable.Bundles); // 30
        ConfigureContainer();
        ConfigDataBase();
    }
    ```

2. Migrate **ConfigureContainer** to ASP.NET Core. The method will need to be rewritten inside of **Program.cs**.

    ```csharp
    // Original Method
    private void ConfigureContainer()
    {
        // This version of AutoFac is not compatible with ASP.NET Core
        var builder = new ContainerBuilder();
        // ConfigurationManager Not compatible with ASP.NET Core
        var mockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]);
        builder.RegisterModule(new ApplicationModule(mockData));
        container = builder.Build();
        _containerProvider = new ContainerProvider(container);
    }

    ```

    * ASP.NET Framework's **System.Configuration.ConfigurationManager** is not compatible with ASP.NET Core's **Microsoft.Extensions.Configuration.ConfigurationManager** and will need to be updated. In **Program.cs** under `CreateBuilder`, write a statement that fetches the value of `UseMockData` from configuration.

    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    // App Settings
    bool useMockData = bool.Parse(builder.Configuration["UseMockData"]!);
    ```

    > Why the bang? The null forgiving operator '!' in the Parse statement ignores the "possibility of null" warning on this statement. If the value is null, it will cause an exception. In this case the exception is desired as it will force the application settings to be set before the application is allowed to run.

    * Autofac will need to be updated to the latest version that supports .NET Core. Open the NuGet package manager, then browse for **Autofac**, and **Autofac.Extensions.DependencyInjection** and install both packages. After the pacakges are installed, add the following using statements to `Program.cs`.

    ```csharp
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using eShopLegacyWebForms.Modules;
    using eShopTelerikBlazorServer.Services;
    ```

    * Use Autofac to re-implement **ConfigureContainer** in **Program.cs**. The new implementation will reuse the existing **ApplicationModule** code that was auto-migrated during the upgrade process. To implement Autofac, write a statement that uses **builder.Host.UseServiceProviderFactory**.

    ```csharp
    // Add services to the container.
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(container =>
            {
                container.RegisterModule(new ApplicationModule(useMockData));
            });
    ```

3. Migrate **ConfigDataBase** to ASP.NET Core. The method will need to be rewritten inside of **Program.cs**.

    ```csharp
    private void ConfigDataBase()
    {
        // ConfigurationManager Not compatible with ASP.NET Core
        // Already migrated in previous step
        var mockData = bool.Parse(ConfigurationManager.AppSettings["UseMockData"]);

        if (!mockData)
        {
            // Requires EF, see notes on EF vs EF Core
            // container.Resolve API has moved to IServiceProvider
            Database.SetInitializer<CatalogDBContext>(container.Resolve<CatalogDBInitializer>());
        }
    }

    ```

    > The **Database.SetInitializer** method and the **CatalogDBInitializer** class make heavy use of EntityFramework (EF). Instead of migrating all of the classes that interact with EF to EF Core, the code can be used with the latest version of EF. Minor updates will be needed for compatibility. The code related to EF can be migrated at a later time. For more information about the benefits of moving to EF Core see the offical documentation. [EF Core and EF6](https://learn.microsoft.com/en-us/ef/efcore-and-ef6/)

    * Install the latest version of **EntityFramework**. Open NuGet Package Manager and search for **EntityFramework**, then install the package.

    * Set the database initializer by adding a using statement for **System.Data.Entity**. Then update the statement `container.Resolve` to use the newer `GetRequiredService` method from the app's services container.

    ```csharp
    var app = builder.Build();

    if (!useMockData)
    {
        Database.SetInitializer(app.Services.GetRequiredService<CatalogDBInitializer>());
    }
    ```

4. The database initializer is now set **CatalogDBInitializer**, however the CatalogDBInitializer class has errors due to breaking changes from ASP.NET Framework to ASP.NET Core. Migrate **CatalogDBInitializer** by updating the code identified by the compiler.

    ```csharp
    // Make sure to copy the files below from the old project
    private const string CatalogItemHiLoSequenceScript = @"Models\Infrastructure\dbo.catalog_hilo.Sequence.sql"; // 17
    private const string CatalogBrandHiLoSequenceScript = @"Models\Infrastructure\dbo.catalog_brand_hilo.Sequence.sql"; // 18
    private const string CatalogTypeHiLoSequenceScript = @"Models\Infrastructure\dbo.catalog_type_hilo.Sequence.sql"; // 19
    ```

    Update the following code. Replace **ConfigurationManager** with **IConfiguration**.

    ```csharp
    //before
    private bool useCustomizationData; // 22

    public CatalogDBInitializer(CatalogItemHiLoGenerator indexGenerator) // 24
    {
        this.indexGenerator = indexGenerator;
        useCustomizationData = bool.Parse(ConfigurationManager.AppSettings["UseCustomizationData"]);
    }
    ```

    * The resulting code should use DI to bring in **IConfiguration** and set the variable **useCustomizationData**

    ```csharp
    //after
    private readonly bool useCustomizationData;

    public CatalogDBInitializer(CatalogItemHiLoGenerator indexGenerator, IConfiguration configuration)
    {
        this.indexGenerator = indexGenerator;
        useCustomizationData = bool.Parse(configuration["UseCustomizationData"]!);
    }
    ```

    * The API **HostingEnvironment.ApplicationPhysicalPath** is used to get the physical path of the running application. This method is not part of **IWebHostEnvironment**. Replace instances of **HostingEnvironment.ApplicationPhysicalPath** with **IWebHostEnvironment**. Update all instances of contentRootPath.

    ```csharp
    private readonly bool useCustomizationData;
    private readonly string contentRootPath;

    public CatalogDBInitializer(CatalogItemHiLoGenerator indexGenerator, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
    {
        this.indexGenerator = indexGenerator;
        useCustomizationData = bool.Parse(configuration["UseCustomizationData"]!);
        contentRootPath = hostingEnvironment.ContentRootPath;
    }
    ```

5. Update the constructor for **CatalogDBContext**  **IConfiguration**. In CatalogDBContext.cs add an IConfiguration parameter to the constructor. Then, pass the connection string from the IConfiguration to the **base** constructor.

    ```csharp
    public CatalogDBContext(IConfigurationconfiguration) : base(configuration.GetConnectionString("CatalogDBCotext"))
    {
    }
    ```

6. **Global.asax.cs** is not used in ASP.NET Core. Remove **Global.asax.cs** from the **eShopTelerikBlazorServer** project.

7. At this point the **eShopTelerikBlazorServer** project should compile without errors. Build and Start the application. The application will fail to start because the application settings have not yet been migrated.

    * In **ASP.NET Framework** application settings were stored in the **Web.config** file. With ASP.NET Core settings are now stored in **appsettings.json**. Migrate the settings from Web.config to appsettings.json.

    appsettings.json
    ```js
    { ... existing settings ... }
    ,
    "UseMockData": true,
    "UseCustomizationData": false,
    "ConnectionStrings": {
        "CatalogDBContext": "Server=(localdb)\\mssqllocaldb;Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb;Trusted_Connection=True;MultipleActiveResultSets=true;"
    }
    ```

    * At this point the **eShopTelerikBlazorServer** project should start without errors. Build and Start the application.

8. Copy static assets from **eShopLegacyWebForms\Pics** to **eShopTelerikBlazorServer\wwwroot\Pics**. In ASP.NET Core, the static assets must be in the **wwwroot** folder to be served to the client. 

9. (optional) Write a quick example to check data connectivity. Replace the code in **Index.razor**. Inject the **ICatalogService** and display data on the page. Try setting the **UseMockData** value to **true** and verify the SQL database is created.

    ```html
    @page "/"
    @using eShopLegacyWebForms.Models;
    @using eShopLegacyWebForms.Services;

    @inject ICatalogService CatalogService

    @foreach (var item in catalogs)
    {
        <p>@item.Name</p>
    }

    @code {
        CatalogItem[]? catalogs;

        protected override Task OnInitializedAsync()
        {

            catalogs = CatalogService.GetCatalogItemsPaginated(10,0).Data.ToArray();
            return base.OnInitializedAsync();
        }
    }
    ```
    
## Conclusion

In this section we continued where the .NET Upgrade Assistant left off. The Global.asax.cs was migrated to Program.cs. Configuration related code was updated to use the new methods of storing and retrieving configuration information. The DI code base was migrated to a new version of Autofac and configured for ASP.NET Core. In addition, some database initialization code was rewritten.

Overall, the "back end" of the application has been migrated successfully. In the next section we'll focus on the "front end" of the application by generating a new view using the Telerik UI for Blazor scaffolding tool.