# Common errors

Error: Unable to find package Telerik.UI.for.Blazor. No packages exist with this id in source(s): {pathName}

Solution: The finished project requires either a Trial or Commercial license of Telerik UI for Blazor. When using a Trial version of Telerik UI for Blazor, the following references must be changed to include the Trial namespace.

**eShopTelerikBlazorServer.csproj**
```
<PackageReference Include="Telerik.UI.for.Blazor.Trial" Version="4.5.0" />
```

Pages\_Host.cshtml
```
<!-- Trial script file -->
<!-- If you are using a commercial license, replace the telerik-blazor.js script tag with the one commented below. -->

<script src="_content/Telerik.UI.for.Blazor.Trial/js/telerik-blazor.js" defer></script>

<!-- /Trial script file -->
<!-- Commercial script file -->
<!--<script src="_content/Telerik.UI.for.Blazor/js/telerik-blazor.js"></script>-->
<!-- /Commercial script file -->
```