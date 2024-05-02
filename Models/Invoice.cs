namespace telerikReportingDemo.Models;

public record Invoice
{
    public int InvoiceId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    public List<InvoiceLine> Lines { get; set; }
    public Invoice()
    {
        Lines = new List<InvoiceLine>();
        Lines.Add(new InvoiceLine()
        {
            Qty = 1,
            Description = "Description Placeholder",
            Discount = 1,
            Sku = "000001",
            UnitPrice = 1.5M
        });

        Lines.Add(new InvoiceLine()
        {
            Qty = 4,
            Description = "Description Placeholder 2",
            Discount = 2,
            Sku = "000002",
            UnitPrice = 4.8M
        });

        this.Lines = Lines;
        this.InvoiceDate = DateTime.Now;
        this.CustomerName = "Mr R Williams";
    }
}
