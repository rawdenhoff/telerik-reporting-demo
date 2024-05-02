namespace telerikReportingDemo.Models;

public record Invoice
{
    public int InvoiceId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    public List<InvoiceLine> Lines { get; set; }
}
