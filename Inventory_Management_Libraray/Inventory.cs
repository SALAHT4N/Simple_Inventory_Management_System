using Inventory_Management_Libraray.interfaces;
using System.Collections.Generic;

namespace Inventory_Management_Library
{
    public class Inventory
    {
        private readonly IProductRepository _productRepository;
        public Inventory(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public ErrorLevels AddProduct(string name, double price, int quantity)
        {
            var product = new Product() { Name = name, Price = price };
            product.IncreaseQuantity(quantity);

            if (_productRepository.AddProduct(product))
                return ErrorLevels.CommandDone;

            return ErrorLevels.ProductAlreadyExists;
        }
        public ErrorLevels RemoveProduct(string name)
        {
            if (_productRepository.RemoveProduct(name))
                return ErrorLevels.CommandDone;

            return ErrorLevels.ProductNotFound;
        }
        public ErrorLevels EditProduct(string name, ProductDetails productDetails)
        {
            var product = new Product() { Name = productDetails.Name, Price = productDetails.Price };
            product.IncreaseQuantity(productDetails.Quantity);

            if (!_productRepository.UpdateProduct(name, product))
            {
                return ErrorLevels.CannotDo;
            }
            return ErrorLevels.CommandDone;
        }
        private Product GetProduct(string name)
        {
            return _productRepository.GetProduct(name);
        }
        public IEnumerable<string> GetAllProducts()
        {
            foreach (var p in _productRepository.GetAllProducts())
            {
                yield return p.GetDetails(DisplayFormat.Short);
            }
        }
        public ErrorLevels CheckProductPresence(string name) =>
            (GetProduct(name) != null) ?
            ErrorLevels.ProductFound :
            ErrorLevels.ProductNotFound;
        
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

    }
}
