using System.Collections.Generic;

namespace Microsoft.eShopWeb.ApplicationCore.Models;
public class OrderDto
{
    public string Id { get; set; }
    public string CountryCity { get; set; }

    public string ShippingAddress { get; set; }
    
    public List<OrderItemDto> OrderItems { get; set; }

    public decimal FinalPrice { get; set; }
}
