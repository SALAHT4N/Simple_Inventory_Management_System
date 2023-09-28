using Inventory_Management_Libraray.interfaces;
using Inventory_Management_Library;
using System.Collections.Generic;

namespace Inventory_Management_Libraray.repos
{
    public class ProductStaticRepository : IProductRepository
    {
        private List<Product> products = new();
        public bool AddProduct(Product product)
        {
            if (CheckProductPresence(product.Name) != ErrorLevels.ProductNotFound)
                return false;

            products.Add(product);
            return true;
        }

        public bool RemoveProduct(string productName)
        {
            Product remove = GetProduct(productName);

            if (remove == null) return false;

            products.Remove(remove);
            return true;
        }

        public bool UpdateProduct(string productName, Product newProduct)
        {
            var editedProduct = GetProduct(productName);
            if (editedProduct == null) return false;

            if (productName != newProduct.Name &&
                CheckEditability(productName, newProduct.Name) == ErrorLevels.ProductAlreadyExists)
            {
                return false;
            }

            editedProduct.Name = newProduct.Name;
            editedProduct.Price = newProduct.Price;
            editedProduct.Consume(editedProduct.Quantity);
            editedProduct.IncreaseQuantity(newProduct.Quantity);

            return true;
        }

        private ErrorLevels CheckEditability(string name, string newName)
        {
            if (GetProduct(newName) != null) return ErrorLevels.ProductAlreadyExists;

            Product edit = GetProduct(name);
            if (edit == null) return ErrorLevels.ProductNotFound;

            return ErrorLevels.ProductFound;
        }
        public ErrorLevels CheckProductPresence(string name) =>
            (GetProduct(name) != null) ?
            ErrorLevels.ProductFound :
            ErrorLevels.ProductNotFound;

        public Product GetProduct(string productName)
        {
            foreach (var p in products)
            {
                if (p.Name == productName) return p;
            }
            return null;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return products;
        }
    }
}
