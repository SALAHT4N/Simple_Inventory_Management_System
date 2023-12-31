﻿using MongoDB.Bson.Serialization.Attributes;

namespace Inventory_Management_Library
{
    public class Product
    {
        [BsonElement("name")]
        public string Name
        {
            get; set;
        } = string.Empty;

        [BsonElement("price")]
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
        [BsonElement("quantity")]
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
            // return a value to indicate error
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
