using telerikReportingDemo.Models;

namespace telerikReportingDemo.Interfaces;

public interface IInvoiceService
{
    Task<Invoice?> GetInvoiceAsync(int InvoiceId);
}
