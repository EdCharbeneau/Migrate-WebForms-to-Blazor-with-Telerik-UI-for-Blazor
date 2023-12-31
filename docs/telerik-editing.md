---
title: 5. Telerik Editing
nav_order: 5
---

# Telerik Grid editing operations

In the original application individual views were used for creating, editing, and deleting data. The Telerik Grid can perform these tasks using one of several built-in editor. Depending on the data model, editing operations may be completely automated by the Telerik Grid. In this scenario, some additional steps are required to customize the experience using templates.

In this section we'll make full use of the Telerik Grid's editing capabilities and editor templates, and events. 

1. Run the application, click the edit button for an item and examine the generated popup editor. Note the **Brand** and **Type** editors are text boxes, however in the model these fields are backed by an Id number.

    * Next, click save. When the application is using EF instead of mock data it will throw an exception in **CatalogService**. This error is due to entity tracking, and caused by changes made to the **GetCatalogItemsPaginated** method in a previous exercise. To resolve the exception, reattach and update the entity before saving.

    ```csharp
    public void UpdateCatalogItem(CatalogItem catalogItem)
    {
        // reattach entity
        var item = db.CatalogItems.Find(catalogItem.Id); 
        // update entity 
        db.Entry(item).CurrentValues.SetValues(catalogItem);
        // Set the item's state to modified 
        db.Entry(item).State = EntityState.Modified;
        db.SaveChanges();
    }
    ```

2. Add a custom editor for CatalogType and CatalogBrand using the GridColumn's **EditorTemplate** feature. In the EditorTemplate use a **TelerikDropDownList**.

    * In the code section add a private field to hold the collection of CatalogType and CatalogBrand. Set the fields to an empty collection. Next, use the OnInitializedAsync method to fetch to populate the fields using the DataService's GetCatalogTypes and GetCatalogBrands methods.

    ```html
    private IEnumerable<CatalogType> types = Array.Empty<CatalogType>();
    private IEnumerable<CatalogBrand> brands = Array.Empty<CatalogBrand>();

    protected override Task OnInitializedAsync()
    {
        types = DataService.GetCatalogTypes();
        brands = DataService.GetCatalogBrands();
        return base.OnInitializedAsync();
    }
    ```

* Update the **GridColumn** for **CategoryBrand** with an **EditorTemplate**. Open a new EditorTemplate, inside use a razor code block to get the CatalogItem and bind its values to a **TelerikDropDownList**. The TelerikDropDownList's bindings should be:
    * Data="@brands"
    * TextField="Brand"
    * ValueField="Id"
    * bind-value="item.CatalogBrandId"
    
    * Repeat the process for the CatalogType column.

    ```html
    <GridColumn Field="CatalogBrand.Brand" Title="Brand" FilterMenuType="@FilterMenuType.CheckBoxList">
        <EditorTemplate>
            @{
                var item = (CatalogItem)context;
                <TelerikDropDownList Data="@brands" TextField="Brand" ValueField="Id" @bind-Value="item.CatalogBrandId"/>
            }
        </EditorTemplate>
    </GridColumn>
    <GridColumn Field="CatalogType.Type" Title="Type" FilterMenuType="@FilterMenuType.CheckBoxList">
        <EditorTemplate>
            @{
                var item = (CatalogItem)context;
                <TelerikDropDownList Data="@types" TextField="Type" ValueField="Id" @bind-Value="item.CatalogTypeId"/>
            }
        </EditorTemplate>
    </GridColumn>
    ```

    * Start the application and try to **edit a record**. The record should submit and update successfully. 

3. Start the application and try to **add** a record. This will cause an exception to occur. When using complex models sometimes the grid will need help initializing an empty record. In this instance, the new record is causing an execption because the CatalogBrand and CatalogType are initializing as null. The Telerik Grid features an OnAdd that fires when the Add command button for a newly added item is clicked.

    * Use the "OnAdd" event to initialize a new CatalogItem when the Add button is clicked.

    ```html
    async Task OnAdd(GridCommandEventArgs args)
    {
        var newItem = (CatalogItem)args.Item;
        //Set default values for new items
        newItem.Name = "Untitled";
        newItem.Description = "Description...";
        newItem.CatalogBrand = brands.First();
        newItem.CatalogType = types.First();
        newItem.CatalogTypeId = types.First().Id;
        newItem.CatalogBrandId = brands.First().Id;
        newItem.PictureUri = "";
        //Cancel if needed
        //args.IsCancelled = true;
    }
    ```

    * Delegate the new method to the OnAdd event on the TelerikGrid.

    ```html
    <TelerikGrid ... OnAdd="@OnAdd">
    ```

    * Start the application and try to **add a record**. The record should submit and update successfully. The new record will appear on the last page of the grid.

3. The Telerik Grid includes a built-in dialog to prevent accidental deletion of records. To enable the dialog simply set the ConfirmDelete property on the grid to `true`.

    * Start the application and try to **delete a record**. The action should cause a prompt confirming the action.

    ```html
    <TelerikGrid ... ConfirmDelete="true">
    ```

## Conclusion

In this section we replaced the create, edit and delete views in the application by leveraging the existing Telerik Grid view. With all the views for the application migrated, the new application is ready for production.
