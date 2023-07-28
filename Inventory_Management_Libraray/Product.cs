using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_Library
{
    internal class Product
    {
        public string Name
        {
            get; set;
        } = string.Empty;

        private double price;
        public double Price
        {
            get 
            {
                return price;
            }
            set
            {
                price = (value > 0) ? value : 0;
            }
        }

        public int Quantity
        {
            get; private set;
        }

        public void IncreaseQuantity(int amount)
        {
            Quantity += amount;
        }
        public void Consume(int amount)
        {
            int amountLeft = Quantity - amount;
            Quantity = (amountLeft >= 0) ? amountLeft : 0;
        }

        public string GetDetails(DisplayFormat type) => (type == DisplayFormat.Full) ? FullDisplay() : ShortDisplay();
      
        private string ShortDisplay() => $"{Name}: {Quantity} in stock";
        private string FullDisplay() =>
            $"| Name: {Name}".PadRight(25) + "|\n" +
            $"| Price: {Price}$".PadRight(25) + "|\n" +
            $"| Stock: {Quantity}".PadRight(25) + "|";
    }
}
