using MongoDB.Bson.Serialization.Attributes;
using SharedMessages.SharedData;

namespace Inventory.Microservice.Data;

public class Inventory : BaseEntity
{
        [BsonElement("Description")]
        public string? Description { get; set; }
        [BsonElement("Price")]
        public Double Price { get; set; }
        [BsonElement("Quantitiy")]
        public int Quantitiy { get; set; }
}