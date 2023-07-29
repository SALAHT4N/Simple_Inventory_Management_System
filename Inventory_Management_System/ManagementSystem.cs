using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory_Management_Library;

namespace Inventory_Management_System
{
    public class ManagementSystem
    {
        static private Inventory inventory = new Inventory();
        public static ErrorLevels AddProduct(ProductDetails details)
        {
            var status = inventory.AddProduct(details.Name, details.Price, details.Quantity);
            return status;
        }
        public static ErrorLevels DeleteProduct(string name)
        {
            var status = inventory.RemoveProduct(name);
            return status;
        }
        public static ErrorLevels EditProduct(string name, ProductDetails newDetails)
        {
            ErrorLevels status1 = ErrorLevels.CommandDone;
            if (newDetails.Name != name)
                status1 = inventory.EditProductName(name, newDetails.Name);
            var status2 = inventory.EditProductPrice(name, newDetails.Price);
            var status3 = inventory.EditProductQuantity(name, newDetails.Quantity);

            return (
                status1 == ErrorLevels.CommandDone &&
                status2 == ErrorLevels.CommandDone &&
                status3 == ErrorLevels.CommandDone 
                ) ? ErrorLevels.CommandDone: ErrorLevels.ProductAlreadyExists;
        }
        public static IEnumerable<string> ListAllProducts() =>
            inventory.GetAllProducts();

        // returns either found or not found.
        public static ErrorLevels SearchForProduct(string name, out string details){
            ErrorLevels level = inventory.CheckProductPresence(name);

            details = null;
            if (level == ErrorLevels.ProductFound)
            {
                details = inventory.GetProductDetails(name);
            }

            return level;
        }

        public static ErrorLevels SearchForProduct(string name, out ProductDetails details)
        {
            ErrorLevels level = inventory.CheckProductPresence(name);

            details = null;
            if (level == ErrorLevels.ProductFound)
            {
                details = inventory.GetProductDetailsRecord(name);
            }

            return level;
        }

    }
}
