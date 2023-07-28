using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple_Inventory_Management_System.Inventory_Management_Library;

namespace Inventory_Management_System
{
    public class ManagementSystem
    {
        static private Inventory inventory = new Inventory();
        public static ErrorLevels AddProduct(string name, double price, int quantity)
        {
            inventory.AddProduct(name, price, quantity);
            return ErrorLevels.CannotDo;
        }
        public static ErrorLevels DeleteProduct(string name)
        {
            return ErrorLevels.CannotDo;

        }
        public static ErrorLevels EditProduct(string name, double price, int quantity)
        {
            return ErrorLevels.CannotDo;

        }
        public static IEnumerable<string> ListAllProducts() =>
            inventory.GetAllProducts();

        public static ErrorLevels SearchForProduct(string name, out string details){
            if (inventory.CheckProductPresence(name) != ErrorLevels.ProductFound)
            {
                details = null;
                return ErrorLevels.ProductNotFound;
            }

            details = inventory.GetProductDetails(name);
            return ErrorLevels.ProductFound;
        }

    }
}
