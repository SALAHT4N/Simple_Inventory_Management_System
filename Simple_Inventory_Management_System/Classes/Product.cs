using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Inventory_Management_System.Classes
{
    public class Product
    {
        public string Name
        {
            get; set;
        } = string.Empty;

        public double Price
        {
            get; set;
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
            $"Name: {Name}\n" +
            $"Price: {Price}$\n" +
            $"Stock: {Quantity}";
    }
}
