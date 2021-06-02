using MongoDB.Driver;

namespace DAL
{
    public class DBContext
    {
        private readonly IMongoDatabase _database = null;
        
        public DBContext(MongoConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(config.Database);
        }

        internal IMongoCollection<NewsEntity> News
        {
            get
            {
                return _database.GetCollection<NewsEntity>("news");
            }
        }
    }
}
