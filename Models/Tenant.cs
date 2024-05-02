namespace telerikReportingDemo.Models;

public class Tenant
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UrlPrefix { get; set; } = string.Empty;
}
