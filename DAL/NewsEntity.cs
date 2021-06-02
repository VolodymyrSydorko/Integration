using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DAL
{
    public class NewsEntity
    {
        [BsonId]
        public ObjectId InternalId { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }

        public bool IsLiked { get; set; }

        [BsonDateTimeOptions]
        public DateTime DateOfPublication { get; set; }
    }
}
