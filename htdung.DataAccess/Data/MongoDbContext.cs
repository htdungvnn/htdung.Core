using htdung.DataAccess.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace htdung.DataAccess.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            // Initialize the Mongo client and database
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        // Returns the collection for a given type T
        public IMongoCollection<T> GetCollection<T>(string collectionName) 
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
