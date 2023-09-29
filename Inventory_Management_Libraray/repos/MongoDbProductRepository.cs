using Inventory_Management_Libraray.interfaces;
using Inventory_Management_Library;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventory_Management_Libraray.repos
{
    public class MongoDbProductRepository : IProductRepository
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<Product> _productsCollection;
        public MongoDbProductRepository()
        {

            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI") ?? "mongodb://localhost:27017";
            if (connectionString == null)
            {
                throw new ArgumentNullException("No MONGODB_URI connection string found in enviroment variables");
            }

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase("Simple_Inventory");
            _productsCollection = _database.GetCollection<Product>("Products");
        }
        public bool AddProduct(Product product)
        {
            var insertState = false;
            try
            {
                _productsCollection.InsertOne(product);
                insertState = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return insertState;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            var idExclude = Builders<Product>.Projection.Exclude("_id");
            return _productsCollection.Find(Builders<Product>.Filter.Empty).Project<Product>(idExclude).ToEnumerable();
        }

        public Product GetProduct(string productName)
        {
            var filter = Builders<Product>.Filter.Eq("name", productName);
            var projection = Builders<Product>.Projection.Exclude("_id");
            var product = _productsCollection
                .Find(filter)
                .Project<Product>(projection)
                .FirstOrDefault();

            return product;
        }

        public bool RemoveProduct(string productName)
        {
            var result = _productsCollection.DeleteOne(p => p.Name == productName);
            return result.DeletedCount == 1;
        }

        public bool UpdateProduct(string productName, Product newProduct)
        {
            var count = _productsCollection.Find(p => p.Name == newProduct.Name).CountDocuments();
            var reserved = count > 0;

            if (reserved) return false;

            var update = Builders<Product>.Update
                .Set("name", newProduct.Name)
                .Set("price", newProduct.Price)
                .Set("quantity", newProduct.Quantity);
            var result = _productsCollection.UpdateOne(p => p.Name == productName, update);

            return result.ModifiedCount > 0;
        }
    }
}
