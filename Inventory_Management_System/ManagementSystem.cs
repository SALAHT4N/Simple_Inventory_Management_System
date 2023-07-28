﻿using System;
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
        public static ErrorLevels AddProduct(string name, double price, int quantity)
        {
            inventory.AddProduct(name, price, quantity);
            return ErrorLevels.CannotDo;
        }
        public static ErrorLevels DeleteProduct(string name)
        {
            return ErrorLevels.CannotDo;

        }
        public static ErrorLevels EditProduct(string name, ProductDetails newDetails)
        {
            inventory.EditProductName(name, newDetails.Name);
            inventory.EditProductPrice(name, newDetails.Price);
            inventory.EditProductQuantity(name, newDetails.Quantity);

            return ErrorLevels.CommandDone;
        }
        public static IEnumerable<string> ListAllProducts() =>
            inventory.GetAllProducts();

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
