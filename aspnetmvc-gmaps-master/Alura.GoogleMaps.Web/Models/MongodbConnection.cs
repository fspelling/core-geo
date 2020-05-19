using MongoDB.Driver;

namespace Alura.GoogleMaps.Web.Models
{
    public class MongodbConnection
    {
        private const string CONN_STRING = "mongodb://localhost:19003";
        private const string DATABASE = "geo";

        public IMongoClient Client { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public IMongoCollection<Aeroporto> Airports => Database.GetCollection<Aeroporto>("airports");

        public MongodbConnection()
        {
            Client = new MongoClient(CONN_STRING);
            Database = Client.GetDatabase(DATABASE);
        }
    }
}