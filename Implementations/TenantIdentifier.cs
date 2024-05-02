using Microsoft.AspNetCore.Http.Extensions;
using telerikReportingDemo.Interfaces;
using telerikReportingDemo.Models;

namespace telerikReportingDemo.Implementations;

public class TenantIdentifier : ITenantIdentifier
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public TenantIdentifier(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Tenant?> GetTenantAsync()
    {
        HttpContext? context = httpContextAccessor.HttpContext;
        var uri = new Uri(context.Request.GetDisplayUrl());
        bool booValidClient = false;
        string strDomain;
        string strPrefix;

        strDomain = context.Request.Host.Host.ToString();
        strPrefix = context.Request.Host.ToString().Split(".").First().ToLower();
        List<string> domainComponents = strDomain.Split(".").ToList();


        switch (strPrefix)
        {
            case "toys":
                return new Tenant() { TenantId = 1, Name = "Lego Toys Inc.", UrlPrefix = strPrefix };
            case "cars":
                return new Tenant() { TenantId = 2, Name = "Second Hand Cars Ltd.", UrlPrefix = strPrefix };
        }
        return null;
    }
}
