
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using Telerik.Reporting;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.WebReportDesigner.Services;
using telerikReportingDemo.Components;
using telerikReportingDemo.Implementations;
using telerikReportingDemo.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages()
     .AddNewtonsoftJson();
builder.Services.AddServerSideBlazor();

builder.Services.TryAddSingleton<IReportServiceConfiguration>(sp => new ReportServiceConfiguration
{
    ReportingEngineConfiguration = sp.GetService<IConfiguration>(),
    HostAppId = "BlazorWebReportDesignerDemo",
    Storage = new FileStorage(),
    ReportSourceResolver = new UriReportSourceResolver(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "Reports"))
});
builder.Services.TryAddSingleton<IReportDesignerServiceConfiguration>(sp => new ReportDesignerServiceConfiguration
{
    DefinitionStorage = new FileDefinitionStorage(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "Reports")),
    SettingsStorage = new FileSettingsStorage(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telerik Reporting")),
    ResourceStorage = new ResourceStorage(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "Resources")),
    SharedDataSourceStorage = new FileSharedDataSourceStorage(Path.Combine(sp.GetService<IWebHostEnvironment>().WebRootPath, "Reports", "Shared Data Sources")),
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantIdentifier, TenantIdentifier>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddTelerikBlazor();

builder.Services.AddSingleton<IReportServiceConfiguration>(sp =>
{
    //var tenantIdentifierServiceFactory = sp.GetRequiredService<ITenantIdentifier>();
    var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var reportsPath = System.IO.Path.Combine(sp.GetService<IWebHostEnvironment>().ContentRootPath, "wwwroot", "Reports");
    //reportsPath = "C:\\Program Files (x86)\\Progress\\Telerik Reporting 2024 Q1\\Report Designer\\Examples";
    return new ReportServiceConfiguration
    {
        ReportingEngineConfiguration = ConfigurationHelper.ResolveConfiguration(sp.GetService<IWebHostEnvironment>()),
        HostAppId = "ReportingCore3App",
        Storage = new FileStorage(),
        ReportSourceResolver = new ReportSourceResolver(reportsPath, serviceScopeFactory),
        //ReportSourceResolver = new UriReportSourceResolver(reportsPath),
        ReportSharingTimeout = 1400
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();




app.UseAntiforgery();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    
});



app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();



app.Run();


static class ConfigurationHelper
{
    public static IConfiguration ResolveConfiguration(IWebHostEnvironment environment)
    {
        var reportingConfigFileName = System.IO.Path.Combine(environment.ContentRootPath, "appsettings.json");
        return new ConfigurationBuilder()
            .AddJsonFile(reportingConfigFileName, true)
            .Build();
    }
}

public class ReportSourceResolver : IReportSourceResolver
{
    Report gridReportInstance;
    string reportPath;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public ReportSourceResolver(
        string reportsPath,
        IServiceScopeFactory serviceScopeFactory
        )
    {
        this.reportPath = reportsPath;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public ReportSource Resolve(string report, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
    {
        try
        {

            FileInfo fiReport = new FileInfo(System.IO.Path.Combine(this.reportPath, report));
            var reportPackager = new ReportPackager();
            using (var sourceStream = File.OpenRead(fiReport.FullName))
            {
                var reportDef = (Report)reportPackager.UnpackageDocument(sourceStream);
                this.gridReportInstance = reportDef;
            }

            var toReturn = new InstanceReportSource()
            {
                ReportDocument = this.gridReportInstance
            };

            var dataSource = this.gridReportInstance.DataSource as ObjectDataSource;
            var scope = serviceScopeFactory.CreateScope();
            var invoiceService = scope.ServiceProvider.GetRequiredService<IInvoiceService>();

            //                         not ideal code if exceptions are generated
            var invoice = invoiceService.GetInvoiceAsync(1).Result;
            dataSource.DataSource = invoice;

            return toReturn;
        }
        catch (Newtonsoft.Json.JsonException jex)
        {
            Trace.TraceError("A problem occurred while deserializing the report source identifier: {0}.{1}Falling back to the next report resolver.", jex, Environment.NewLine);
            return null;
        }
    }

}