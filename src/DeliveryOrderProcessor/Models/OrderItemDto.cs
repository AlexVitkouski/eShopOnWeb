namespace DeliveryOrderProcessor.Models;
public class OrderItemDto
{
    public string Id { get; set; }
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
