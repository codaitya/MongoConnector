using System;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Text;
using System.ComponentModel;

using MongoDB.Driver.Linq;
using System.IO;

namespace MongoResultWrapper
{
    public class MongoResultWrapper
    {
        public String  databaseConnectionString { get; set; }
        public MongoClient mongoClient;

        public MongoResultWrapper (String connectionString)
        {
            databaseConnectionString = connectionString;
        }

        public void  connectToMongo() {
            mongoClient = new MongoClient(databaseConnectionString);
        }

        public async Task<String> executeFind(String findParameter, String database, String collection, String filePath = null) {
            if (mongoClient == null) {
                connectToMongo();
            }
            var db = mongoClient.GetDatabase(database);
            var collectionObject = db.GetCollection<BsonDocument>(collection);

            BsonDocument doc = MongoDB.Bson.Serialization
                   .BsonSerializer.Deserialize<BsonDocument>(findParameter);

            String result = "";
            await collectionObject.Find(doc).ForEachAsync(song =>
            {
                result += song;
                Console.Write(song);
            });

            if (filePath != null)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.WriteLine(result);
                sw.Close();
            }
            return result;
        }
        public async void  executeInsert(String bsonString, String database, String collection)
        {
            if (mongoClient == null)
            {
                connectToMongo();
            }
            var db = mongoClient.GetDatabase(database);
            var collectionObject = db.GetCollection<BsonDocument>(collection);

            BsonDocument doc = MongoDB.Bson.Serialization
                   .BsonSerializer.Deserialize<BsonDocument>(bsonString);

            BsonDocument[] seedData = { doc };

            await collectionObject.InsertManyAsync(seedData);
        }

    }
}
