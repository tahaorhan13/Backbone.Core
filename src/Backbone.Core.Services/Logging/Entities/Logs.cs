using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backbone.Core.Services.Logging.Entities
{
    public class Logs
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; } // Error, Warning, Info
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
