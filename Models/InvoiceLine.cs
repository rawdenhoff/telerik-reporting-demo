namespace telerikReportingDemo.Models;

public record InvoiceLine
{
    public string Sku { get; set; }
    public string Description { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal LinePrice { get => (UnitPrice * Qty) - Discount; }
}
