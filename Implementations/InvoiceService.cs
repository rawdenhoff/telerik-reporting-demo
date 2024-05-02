using System.Reflection.Metadata.Ecma335;
using telerikReportingDemo.Interfaces;
using telerikReportingDemo.Models;

namespace telerikReportingDemo.Implementations;

public class InvoiceService : IInvoiceService
{
    private readonly ITenantIdentifier tenantIdentifier;

    public InvoiceService(ITenantIdentifier tenantIdentifier)
    {
        this.tenantIdentifier = tenantIdentifier;
    }
    public async Task<Invoice?> GetInvoiceAsync(int InvoiceId)
    {
        Tenant? tenant = await tenantIdentifier.GetTenantAsync();

        if (tenant is null)
        {
            throw new Exception("Invalid Tenant");
        }

        switch (tenant.TenantId)
        {
            case 1:
                return GetCarsInvoice();
            case 2:
                return GetToysInvoice();
        }
        return null;
    }

    private Invoice GetCarsInvoice()
    {

        List<InvoiceLine> lines = new List<InvoiceLine>();

        lines.Add(new InvoiceLine() { Qty = 3, Sku = "000002", UnitPrice = 0.2M, Discount = 0, Description = "BOLT" });
        lines.Add(new InvoiceLine() { Qty = 8, Sku = "000052", UnitPrice = 28.5M, Discount = 2.5M, Description = "MIRROR" });

        return new Invoice() 
        { 
            CustomerName = "Mr A. Smith",
            InvoiceDate = DateTime.Now,
            InvoiceId = 1,
            Lines = lines
        };
    }

    private Invoice GetToysInvoice()
    {

        List<InvoiceLine> lines = new List<InvoiceLine>();

        lines.Add(new InvoiceLine() { Qty = 3, Sku = "000021", UnitPrice = 0.2M, Discount = 0.30M, Description = "BATMAN" });
        lines.Add(new InvoiceLine() { Qty = 2, Sku = "000089", UnitPrice = 0.45M, Discount = 2.5M, Description = "MARIO" });
        lines.Add(new InvoiceLine() { Qty = 1, Sku = "000093", UnitPrice = 0.3M, Discount = 0, Description = "PRINCESS PEACH" });

        return new Invoice()
        {
            CustomerName = "Mr I. Rogers",
            InvoiceDate = DateTime.Now,
            InvoiceId = 388,
            Lines = lines
        };
    }

}
