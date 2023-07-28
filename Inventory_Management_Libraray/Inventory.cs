using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_Library
{
    public class Inventory
    {
        private List<Product> products = new();

        private Product GetProduct(string name)
        {
            foreach (var p in products)
            {
                if (p.Name == name) return p;
            }
            return null;
        }

        public ErrorLevels AddProduct(string name, double price, int quantity)
        {
            if (CheckProductPresence(name) != ErrorLevels.ProductNotFound) 
                return ErrorLevels.ProductAlreadyExists;

            Product addedProduct = new() { Name = name, Price = price };
            products.Add(addedProduct);
            addedProduct.IncreaseQuantity(quantity);

            return ErrorLevels.CommandDone;
        }
        public ErrorLevels RemoveProduct(string name)
        {
            Product remove = GetProduct(name);

            if (remove == null) return ErrorLevels.ProductNotFound;

            products.Remove(remove);
            return ErrorLevels.CommandDone;
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
        
        public ErrorLevels EditProductQuantity(string name, int quantity)
        {
            Product edit = GetProduct(name);
            if (edit == null) return ErrorLevels.ProductNotFound;

            edit.Consume(edit.Quantity);
            edit.IncreaseQuantity(quantity);

            return ErrorLevels.CommandDone;
        }
        public ErrorLevels EditProductPrice(string name, double price)
        {
            Product edit = GetProduct(name);
            if (edit == null) return ErrorLevels.ProductNotFound;
            edit.Price = price;

            return ErrorLevels.CommandDone;
        }
        public ErrorLevels EditProductName(string name, string newName)
        {
            ErrorLevels level = CheckEditability(name, newName);
            if (level != ErrorLevels.ProductFound) return level;

            Product edit = GetProduct(name);
            edit.Name = newName;

            return ErrorLevels.ProductFound;
        }
        public string GetProductDetails(string name) =>
            GetProduct(name).GetDetails(DisplayFormat.Full);
        public ProductDetails GetProductDetailsRecord(string name)
        {
            var p = GetProduct(name);
            return new ProductDetails(p.Name, p.Price, p.Quantity);

        }

        public double GetProductPrice(string name) =>
            GetProduct(name).Price;
        public int GetProductQuantity(string name) =>
            GetProduct(name).Quantity;

        public IEnumerable<string> GetAllProducts()
        {
            foreach(var p in products)
            {
                yield return p.GetDetails(DisplayFormat.Short);
            }
        }
    }
}
