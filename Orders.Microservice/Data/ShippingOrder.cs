using MongoDB.Bson.Serialization.Attributes;
using SharedMessages.SharedData;

namespace Orders.Microservice.Data;

public class ShippingOrder : BaseEntity
{
    [BsonElement("Status")]
    public string? Status { get; set; }
}