using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeliveryOrderProcessor.Models;

//todo: how to have only one model for Web and Azure trigger
public class OrderDto
{
    [JsonProperty("id")]
    public string Id { get; set; }
    public string CountryCity { get; set; }
    public string ShippingAddress { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public decimal FinalPrice { get; set; }
}
