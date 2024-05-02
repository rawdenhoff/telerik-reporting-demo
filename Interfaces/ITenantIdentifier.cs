using telerikReportingDemo.Models;

namespace telerikReportingDemo.Interfaces;

public interface ITenantIdentifier
{
    Task<Tenant?> GetTenantAsync();
}
