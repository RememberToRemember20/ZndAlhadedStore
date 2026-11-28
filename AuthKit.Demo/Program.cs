using AuthKit.Blazor.Configuration;
using AuthKit.Blazor.Extensions;
using AuthKit.Demo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddAuthKitUi(o =>
{
    o.ApiBaseAddress = "https://localhost:7293/";      // عنوان AuthKit.Api الفعلي
    o.Theme.AppName = "متجر زند الحديد";
    o.Theme.LogoUrl = "images/logo.svg";
    o.Theme.Primary = "#0f766e";
    o.Theme.Motion = AkMotion.Full;
    // ...الإعدادات الموجودة
    o.Register.Fields.Insert(0, new AkFieldSpec
    {
        Key = "fullName",
        LabelKey = "field.full_name",
        Autocomplete = "name",
        MaxLength = 100
    });
});
await builder.Build().RunAsync();
