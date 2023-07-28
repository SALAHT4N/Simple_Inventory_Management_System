using System;
using Simple_Inventory_Management_System.Inventory_Management_Library;

namespace Inventory_Management_System
{
    public class Program
    {
        
        private static void PrintTitleLogo()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(@"
  _____                      _                     __  __                                                   _      _____           _                 
 |_   _|                    | |                   |  \/  |                                                 | |    / ____|         | |                
   | |  _ ____   _____ _ __ | |_ ___  _ __ _   _  | \  / | __ _ _ __   __ _  __ _  ___ _ __ ___   ___ _ __ | |_  | (___  _   _ ___| |_ ___ _ __ ___  
   | | | '_ \ \ / / _ \ '_ \| __/ _ \| '__| | | | | |\/| |/ _` | '_ \ / _` |/ _` |/ _ \ '_ ` _ \ / _ \ '_ \| __|  \___ \| | | / __| __/ _ \ '_ ` _ \ 
  _| |_| | | \ V /  __/ | | | || (_) | |  | |_| | | |  | | (_| | | | | (_| | (_| |  __/ | | | | |  __/ | | | |_   ____) | |_| \__ \ ||  __/ | | | | |
 |_____|_| |_|\_/ \___|_| |_|\__\___/|_|   \__, | |_|  |_|\__,_|_| |_|\__,_|\__, |\___|_| |_| |_|\___|_| |_|\__| |_____/ \__, |___/\__\___|_| |_| |_|
                                            __/ |                            __/ |                                        __/ |                      
                                           |___/                            |___/                                        |___/                       
");
            Console.ResetColor();
        }

        private enum MainMenuCommands
        {
            ListProducts = 1, AddProduct
        }

        private static void PrintMainMenu()
        {
            Console.WriteLine("Choose a command: ");
            Console.WriteLine($"{(int)MainMenuCommands.ListProducts}. List All Products.");
            Console.WriteLine($"{(int)MainMenuCommands.AddProduct}. Add a Product.");
            Console.WriteLine("");
            // Add product
            // List all products
            // delete product
            // edit product
            // search for product

        }
 
        private static void PrintProductsAndMenu()
        {
            var productNames = ManagementSystem.ListAllProducts();
            foreach (string i in productNames)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine(@"--------------------------");
            Console.Write("Enter a product name to view options: ");
        }

        private static void PrintCommandNotFoundMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Command not found! ");
            Console.ResetColor();
        }
        private static void Init()
        {
            // TODO Add placeholder products to inventory.
            ManagementSystem.AddProduct("cola", 2.5, 5);
            ManagementSystem.AddProduct("pepsi", 2, 6);
            ManagementSystem.AddProduct("cola2", 2.5, 5);
            ManagementSystem.AddProduct("cola3", 2.5, 5);
            ManagementSystem.AddProduct("cola4", 2.5, 5);
            ManagementSystem.AddProduct("cola5", 2.5, 5);

        }
        private static void Run()
        {
            int choice = 0;
            while (true)
            {
                PrintMainMenu();
                int.TryParse(Console.ReadLine().Split(' ')[0], out choice);

                switch (choice)
                {
                    case 1:
                        PrintProductsAndMenu();
                        break;
                    case 2:
                        PrintCreateNewProduct();
                        break;
                    default:
                        PrintCommandNotFoundMessage();
                        break;
                }
                
                
            }
        }

        private static void PrintCreateNewProduct()
        {
            throw new NotImplementedException();
        }

        public static void Main(string[] args)
        {
            Init();
            PrintTitleLogo();
            Run();

        }
    }
}
