using Inventory_Management_Library;
using System.Collections.Generic;

namespace Inventory_Management_Libraray.interfaces
{
    public interface IProductRepository
    {
        bool AddProduct(Product product);
        bool RemoveProduct(string productName);
        bool UpdateProduct(string productName, Product newProduct);
        Product GetProduct(string productName);
        IEnumerable<Product> GetAllProducts();
    }
}
