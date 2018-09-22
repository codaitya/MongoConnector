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

        public String executeFind(String findParameter, String database, String collection, String filePath = null) {
            if (mongoClient == null) {
                connectToMongo();
            }
            var db = mongoClient.GetDatabase(database);
            var collectionObject = db.GetCollection<BsonDocument>(collection);

            BsonDocument doc = MongoDB.Bson.Serialization
                   .BsonSerializer.Deserialize<BsonDocument>(findParameter);

            String result = "";
            System.Collections.Generic.List<BsonDocument> listOfDocuments = collectionObject.FindSync(doc).ToList();

            result = listOfDocuments.ToJson().ToString();

            if (filePath != null)
            {
                StreamWriter sw = new StreamWriter(filePath, false);
                sw.WriteLine(result);
                sw.Close();
            }
            return result;
        }

        public void  executeInsert(String bsonString, String database, String collection)
        {
            if (mongoClient == null)
            {
                connectToMongo();
            }
            var db = mongoClient.GetDatabase(database);
            var collectionObject = db.GetCollection<BsonDocument>(collection);

            BsonDocument [] doc = MongoDB.Bson.Serialization
                   .BsonSerializer.Deserialize<BsonDocument[]>(bsonString);
                                         

            collectionObject.InsertMany(doc);
        }

    }
}
