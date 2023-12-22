using MongoDB.Bson.Serialization.Attributes;
using SharedMessages.SharedData;

namespace Payments.Microservice.Data;

public class Payment : BaseEntity
{
    [BsonElement("DateTime")]
    public DateTime? DateTime { get; set; }
    [BsonElement("Status")]
    public string? Status { get; set; }
    
}