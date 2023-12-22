using MongoDB.Bson.Serialization.Attributes;
using SharedMessages.SharedData;

namespace Orders.Microservice.Data;

public class Order : BaseEntity
{
    [BsonElement("Status")]
    public string? Status { get; set; }
    [BsonElement("Type")]
    public string? Type { get; set; }
    [BsonElement("Datetime")]
    public DateTime? DateTime { get; set; }
}