using System;
using Inventory_Management_Library;
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
            ListProducts = 1, AddProduct, SearchProducts, SelectProduct, Exit = 0,
        }

        private static void MainMenu()
        {
            
            Console.WriteLine("");
            Console.WriteLine($"{(int)MainMenuCommands.ListProducts}. List All Products.");
            Console.WriteLine($"{(int)MainMenuCommands.AddProduct}. Add a Product.");
            Console.WriteLine($"{(int)MainMenuCommands.SearchProducts}. Search for a Product.");
            Console.WriteLine($"{(int)MainMenuCommands.SelectProduct}. Select a Product.");

            Console.WriteLine($"{(int)MainMenuCommands.Exit}. Exit.");
            Console.Write("Choose a command: ");
        }
 
        private static void PrintProductsAndMenu()
        {
            var productNames = ManagementSystem.ListAllProducts();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (string i in productNames)
            {
                Console.WriteLine(i);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void PrintCommandNotFoundMessage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Command not found! ");
            Console.ResetColor();
        }        
        private static void PrintCreateNewProduct()
        {
            throw new NotImplementedException();
        }

        private static ErrorLevels SearchForProductMenu(out string input)
        {
            input = Console.ReadLine();
            ErrorLevels status = ManagementSystem.SearchForProduct(input,out string details);

            if (status == ErrorLevels.ProductFound)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                System.Text.StringBuilder line = new();
                for (int i = 0; i <= 26; i++) line.Append("-");

                Console.WriteLine(line.ToString());
                Console.WriteLine(details);
                Console.WriteLine(line.ToString());

                Console.ResetColor();

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nProduct Not Found!\n");
                Console.ResetColor();
            }
            return status;
        }

        private static void ViewOptions()
        {
            Console.WriteLine($"{(int)ProductOptions.EditProduct}. Edit.");
            Console.WriteLine($"{(int)ProductOptions.DeleteProduct}. Delete.");
            Console.WriteLine($"{(int)ProductOptions.Exit}. go back.");
        }

        private enum ProductOptions
        {
            EditProduct = 1, DeleteProduct, Exit = 0
        }

        private static void EditProductMenu()
        {
            Console.WriteLine("Enter values separated by commas (item,10,_)\t'$' No Change.");
        }
        private static void EditProduct(string selectedProductName)
        {
            EditProductMenu();
            ManagementSystem.SearchForProduct(selectedProductName, out ProductDetails details);
            
            string input = Console.ReadLine();
            var values = input.Split(',');

            if (values.Length < 3) return;

            var newDetails = new ProductDetails(
                (values[0] == "$") ? details.Name : values[0],
                (values[1] == "$") ? details.Price : double.Parse(values[1]),
                (values[2] == "$") ? details.Quantity : int.Parse(values[2])
            );

            ManagementSystem.EditProduct(selectedProductName, newDetails);
        }
        private static void DeleteProductMenu()
        {

        }
        private static void DeleteProduct(string name)
        {
            DeleteProductMenu();
        }
        private static void SelectProductMenu()
        {
            Console.WriteLine("Enter Product Name to view options: ");

            var status = Program.SearchForProductMenu(out var selectedProductName);

            if (status == ErrorLevels.ProductNotFound) return;
            
            Program.ViewOptions();
            Console.Write("Choose an option: ");

            int.TryParse(Console.ReadLine(), out int input);
            if (input == (int)ProductOptions.EditProduct)
            {
                EditProduct(selectedProductName);

            } else if (input == (int)ProductOptions.DeleteProduct)
            {
                DeleteProduct(selectedProductName);
            } else if (input == (int) ProductOptions.Exit)
            {
                return;
            } else
            {
                PrintCommandNotFoundMessage();
                // recall the function we're in
                SelectProductMenu();
            }
        }

        private static void Run()
        {
            int choice = 0;
            while (true)
            {
                MainMenu();
                int.TryParse(Console.ReadLine().Split(' ')[0], out choice);

                switch (choice)
                {
                    case 1:
                        PrintProductsAndMenu();
                        break;
                    case 2:
                        PrintCreateNewProduct();
                        break;
                    case 3:
                        SearchForProductMenu(out string _);
                        break;
                    case 4:
                        SelectProductMenu();
                        ViewOptions();
                        break;

                    default:
                        PrintCommandNotFoundMessage();
                        break;
                }
            }
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
        public static void Main(string[] args)
        {
            Init();
            PrintTitleLogo();
            Run();

        }
    }
}
