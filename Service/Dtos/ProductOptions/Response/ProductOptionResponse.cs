using System;

namespace ProPayments.Service.Dtos.ProductOptions.Response;

public class ProductOptionResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public int Period { get; set; }
    public string PeriodDescription { get; set; } = string.Empty;
}
