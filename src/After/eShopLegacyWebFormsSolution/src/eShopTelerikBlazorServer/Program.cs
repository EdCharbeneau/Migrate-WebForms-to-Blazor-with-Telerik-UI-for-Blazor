#nullable enable
using Autofac;
using Autofac.Extensions.DependencyInjection;
using eShopLegacyWebForms.Models.Infrastructure;
using eShopLegacyWebForms.Modules;
using eShopTelerikBlazorServer.Services;
using System.Data.Entity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// App Settings
bool useMockData = builder.Configuration.GetValue("UseMockData", true);

// Add services to the container.
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>(container =>
        {
            container.RegisterModule(new ApplicationModule(useMockData));
        });


var app = builder.Build();

if (!useMockData)
{
    Database.SetInitializer(app.Services.GetRequiredService<CatalogDBInitializer>());
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
